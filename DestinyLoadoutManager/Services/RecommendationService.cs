using DestinyLoadoutManager.Data;
using DestinyLoadoutManager.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DestinyLoadoutManager.Services
{
    public class RecommendationRequest
    {
        public ElementType ActiveSurge { get; set; }
        public List<int> SelectedChampionIds { get; set; } = new List<int>();
    }

    public class LoadoutRecommendation
    {
        public Loadout? Loadout { get; set; }
        public int MatchScore { get; set; }
        public List<string> MatchReasons { get; set; } = new List<string>();
    }

    public interface IRecommendationService
    {
        Task<List<LoadoutRecommendation>> GetRecommendedLoadoutsAsync(
            string userId,
            RecommendationRequest request);
    }

    public class RecommendationService : IRecommendationService
    {
        private readonly ApplicationDbContext _context;

        public RecommendationService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<LoadoutRecommendation>> GetRecommendedLoadoutsAsync(
            string userId,
            RecommendationRequest request)
        {
            var userLoadouts = await _context.Loadouts
                .Where(l => l.UserId == userId)
                .Include(l => l.LoadoutWeapons)
                .ThenInclude(lw => lw.Weapon)
                .ToListAsync();

            var champions = await _context.Champions
                .Include(c => c.ChampionWeaponTypes)
                .Where(c => request.SelectedChampionIds.Contains(c.Id))
                .ToListAsync();

            var recommendations = new List<LoadoutRecommendation>();

            foreach (var loadout in userLoadouts)
            {
                var recommendation = new LoadoutRecommendation { Loadout = loadout };

                // Calculate match score based on surge match
                var weaponsWithActiveSurge = loadout.LoadoutWeapons
                    .Where(lw => lw.Weapon != null && lw.Weapon.Element == request.ActiveSurge)
                    .ToList();

                if (weaponsWithActiveSurge.Any())
                {
                    recommendation.MatchScore += 10;
                    recommendation.MatchReasons.Add(
                        $"{weaponsWithActiveSurge.Count} weapon(s) match active surge ({request.ActiveSurge})");
                }

                // Calculate champion coverage
                var championCoverageScore = CalculateChampionCoverage(loadout, champions);
                recommendation.MatchScore += championCoverageScore.Score;
                recommendation.MatchReasons.AddRange(championCoverageScore.Reasons);

                // Only include loadouts with some match
                if (recommendation.MatchScore > 0)
                {
                    recommendations.Add(recommendation);
                }
            }

            // Sort by match score (highest first)
            return recommendations.OrderByDescending(r => r.MatchScore).ToList();
        }

        private (int Score, List<string> Reasons) CalculateChampionCoverage(
            Loadout loadout,
            List<Champion> selectedChampions)
        {
            var score = 0;
            var reasons = new List<string>();

            foreach (var champion in selectedChampions)
            {
                var effectiveWeaponTypes = champion.ChampionWeaponTypes
                    .Select(cwt => cwt.WeaponType)
                    .ToList();

                var matchingWeapons = loadout.LoadoutWeapons
                    .Where(lw => lw.Weapon != null && effectiveWeaponTypes.Contains(lw.Weapon.Type))
                    .ToList();

                if (matchingWeapons.Any())
                {
                    score += 5 * matchingWeapons.Count;
                    reasons.Add($"✓ {champion.Name}: {matchingWeapons.Count} suitable weapon(s)");
                }
                else
                {
                    reasons.Add($"✗ {champion.Name}: No suitable weapons");
                }
            }

            return (score, reasons);
        }
    }
}

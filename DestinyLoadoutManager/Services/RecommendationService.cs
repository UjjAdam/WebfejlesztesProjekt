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

                // Surge coverage: weight 30% of total score
                var weaponsWithActiveSurge = loadout.LoadoutWeapons
                    .Where(lw => lw.Weapon != null && lw.Weapon.Element == request.ActiveSurge)
                    .ToList();

                var totalWeapons = loadout.LoadoutWeapons.Count;
                var surgeCoverageRatio = totalWeapons > 0
                    ? Math.Min(1.0, weaponsWithActiveSurge.Count / (double)totalWeapons)
                    : 0;

                var surgeScore = (int)Math.Round(surgeCoverageRatio * 30);
                recommendation.MatchScore += surgeScore;

                if (weaponsWithActiveSurge.Any())
                {
                    recommendation.MatchReasons.Add(
                        $"{weaponsWithActiveSurge.Count}/{totalWeapons} weapons match active surge ({request.ActiveSurge})");
                }
                else
                {
                    recommendation.MatchReasons.Add($"No weapons match active surge ({request.ActiveSurge})");
                }

                // Champion coverage: weight 70% of total score
                var championCoverage = CalculateChampionCoverage(loadout, champions);
                var championRatio = champions.Any()
                    ? (championCoverage.MatchedChampionCount / (double)champions.Count)
                    : 0;
                var championScore = champions.Any()
                    ? (int)Math.Round(championRatio * 70)
                    : 0;

                recommendation.MatchScore += championScore;
                recommendation.MatchReasons.AddRange(championCoverage.Reasons);

                if (champions.Any())
                {
                    recommendation.MatchReasons.Add($"Covers {championCoverage.MatchedChampionCount} / {champions.Count} champion types selected");
                }

                // Always include the loadout so we can still surface the best partial match
                recommendations.Add(recommendation);
            }

            // Sort by match score (highest first)
            return recommendations.OrderByDescending(r => r.MatchScore).ToList();
        }

        private (int MatchedChampionCount, List<string> Reasons) CalculateChampionCoverage(
            Loadout loadout,
            List<Champion> selectedChampions)
        {
            var matchedChampionCount = 0;
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
                    matchedChampionCount++;
                    reasons.Add($"✓ {champion.Name}: {matchingWeapons.Count} suitable weapon(s)");
                }
                else
                {
                    reasons.Add($"✗ {champion.Name}: No suitable weapons");
                }
            }

            return (matchedChampionCount, reasons);
        }
    }
}

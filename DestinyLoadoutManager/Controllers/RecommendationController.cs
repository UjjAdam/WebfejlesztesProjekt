using DestinyLoadoutManager.Data;
using DestinyLoadoutManager.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DestinyLoadoutManager.Controllers
{
    [Authorize]
    public class RecommendationController : Controller
    {
        private readonly IRecommendationService _recommendationService;
        private readonly ApplicationDbContext _context;

        public RecommendationController(
            IRecommendationService recommendationService,
            ApplicationDbContext context)
        {
            _recommendationService = recommendationService;
            _context = context;
        }

        // GET: Recommendation/Index
        public async Task<IActionResult> Index()
        {
            ViewBag.Surges = await _context.Surges.ToListAsync();
            ViewBag.Champions = await _context.Champions.ToListAsync();
            return View();
        }

        // POST: Recommendation/GetRecommendations
        [HttpPost]
        public async Task<IActionResult> GetRecommendations(
            string surgeName,
            List<int> selectedChampionIds)
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? string.Empty;

            var surge = await _context.Surges
                .FirstOrDefaultAsync(s => s.Name == surgeName);

            if (surge == null)
            {
                return BadRequest("Invalid surge selected");
            }

            var request = new RecommendationRequest
            {
                ActiveSurge = surge.ElementType,
                SelectedChampionIds = selectedChampionIds
            };

            var recommendations = await _recommendationService
                .GetRecommendedLoadoutsAsync(userId, request);

            return PartialView("_RecommendationResults", recommendations);
        }
    }
}

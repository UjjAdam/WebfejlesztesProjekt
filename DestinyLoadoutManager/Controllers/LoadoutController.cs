using DestinyLoadoutManager.Data;
using DestinyLoadoutManager.Models;
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
    public class LoadoutController : Controller
    {
        private readonly ILoadoutService _loadoutService;
        private readonly IWeaponService _weaponService;
        private readonly ApplicationDbContext _context;

        public LoadoutController(
            ILoadoutService loadoutService,
            IWeaponService weaponService,
            ApplicationDbContext context)
        {
            _loadoutService = loadoutService;
            _weaponService = weaponService;
            _context = context;
        }

        // GET: Loadout/Index
        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? string.Empty;
            var loadouts = await _loadoutService.GetUserLoadoutsAsync(userId);
            return View(loadouts);
        }

        // GET: Loadout/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? string.Empty;
            var loadout = await _loadoutService.GetLoadoutByIdAsync(id, userId);

            if (loadout == null)
                return NotFound();

            return View(loadout);
        }

        // GET: Loadout/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Loadout/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Description")] Loadout loadout)
        {
            // Set UserId before validation
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? string.Empty;
            loadout.UserId = userId;
            
            // Remove UserId from ModelState validation since we set it manually
            ModelState.Remove("UserId");
            
            if (ModelState.IsValid)
            {
                await _loadoutService.CreateLoadoutAsync(loadout);
                return RedirectToAction(nameof(Edit), new { id = loadout.Id });
            }
            return View(loadout);
        }

        // GET: Loadout/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? string.Empty;
            var loadout = await _loadoutService.GetLoadoutByIdAsync(id, userId);

            if (loadout == null)
                return NotFound();

            ViewBag.PrimaryWeapons = await _weaponService.GetWeaponsBySlotAsync(EquipSlot.Primary);
            ViewBag.SpecialWeapons = await _weaponService.GetWeaponsBySlotAsync(EquipSlot.Special);
            ViewBag.HeavyWeapons = await _weaponService.GetWeaponsBySlotAsync(EquipSlot.Heavy);

            return View(loadout);
        }

        // POST: Loadout/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description")] Loadout loadout)
        {
            if (id != loadout.Id)
                return NotFound();

            // Ensure UserId is set for validation/authorization
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? string.Empty;
            loadout.UserId = userId;
            ModelState.Remove("UserId");

            if (ModelState.IsValid)
            {
                await _loadoutService.UpdateLoadoutAsync(loadout, userId);
                return RedirectToAction(nameof(Details), new { id = loadout.Id });
            }

            ViewBag.PrimaryWeapons = await _weaponService.GetWeaponsBySlotAsync(EquipSlot.Primary);
            ViewBag.SpecialWeapons = await _weaponService.GetWeaponsBySlotAsync(EquipSlot.Special);
            ViewBag.HeavyWeapons = await _weaponService.GetWeaponsBySlotAsync(EquipSlot.Heavy);

            return View(loadout);
        }

        // GET: Loadout/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? string.Empty;
            var loadout = await _loadoutService.GetLoadoutByIdAsync(id, userId);

            if (loadout == null)
                return NotFound();

            return View(loadout);
        }

        // POST: Loadout/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? string.Empty;
            await _loadoutService.DeleteLoadoutAsync(id, userId);
            return RedirectToAction(nameof(Index));
        }

        // POST: Loadout/AddWeapon
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddWeapon(int loadoutId, int weaponId, string slotStr)
        {
            if (System.Enum.TryParse<EquipSlot>(slotStr, out var slot))
            {
                var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? string.Empty;
                await _loadoutService.AddWeaponToLoadoutAsync(loadoutId, weaponId, slot, userId);
            }

            return RedirectToAction(nameof(Edit), new { id = loadoutId });
        }

        // POST: Loadout/RemoveWeapon
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveWeapon(int loadoutWeaponId, int loadoutId)
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? string.Empty;
            await _loadoutService.RemoveWeaponFromLoadoutAsync(loadoutWeaponId, userId);
            return RedirectToAction(nameof(Edit), new { id = loadoutId });
        }
    }
}

using DestinyLoadoutManager.Models;
using DestinyLoadoutManager.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DestinyLoadoutManager.Controllers
{
    [Authorize]
    public class WeaponController : Controller
    {
        private readonly IWeaponService _weaponService;

        public WeaponController(IWeaponService weaponService)
        {
            _weaponService = weaponService;
        }

        // GET: Weapon/Index (Read-only for all users)
        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            var weapons = await _weaponService.GetAllWeaponsAsync();
            return View(weapons);
        }

        // GET: Weapon/Details/5 (Read-only)
        [AllowAnonymous]
        public async Task<IActionResult> Details(int id)
        {
            var weapon = await _weaponService.GetWeaponByIdAsync(id);
            if (weapon == null)
                return NotFound();

            return View(weapon);
        }

        // GET: Weapon/Create (Admin only)
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Weapon/Create (Admin only)
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([Bind("Name,Type,Element,Slot,AmmoType")] Weapon weapon)
        {
            if (ModelState.IsValid)
            {
                await _weaponService.CreateWeaponAsync(weapon);
                return RedirectToAction(nameof(Index));
            }
            return View(weapon);
        }

        // GET: Weapon/Edit/5 (Admin only)
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id)
        {
            var weapon = await _weaponService.GetWeaponByIdAsync(id);
            if (weapon == null)
                return NotFound();

            return View(weapon);
        }

        // POST: Weapon/Edit/5 (Admin only)
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Type,Element,Slot,AmmoType")] Weapon weapon)
        {
            if (id != weapon.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                await _weaponService.UpdateWeaponAsync(weapon);
                return RedirectToAction(nameof(Index));
            }
            return View(weapon);
        }

        // GET: Weapon/Delete/5 (Admin only)
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var weapon = await _weaponService.GetWeaponByIdAsync(id);
            if (weapon == null)
                return NotFound();

            return View(weapon);
        }

        // POST: Weapon/Delete/5 (Admin only)
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _weaponService.DeleteWeaponAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}

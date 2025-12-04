using DestinyLoadoutManager.Data;
using DestinyLoadoutManager.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DestinyLoadoutManager.Services
{
    public interface ILoadoutService
    {
        Task<List<Loadout>> GetUserLoadoutsAsync(string userId);
        Task<Loadout?> GetLoadoutByIdAsync(int id, string userId);
        Task<Loadout> CreateLoadoutAsync(Loadout loadout);
        Task<bool> UpdateLoadoutAsync(Loadout loadout, string userId);
        Task<bool> DeleteLoadoutAsync(int id, string userId);
        Task<bool> AddWeaponToLoadoutAsync(int loadoutId, int weaponId, EquipSlot slot, string userId);
        Task<bool> RemoveWeaponFromLoadoutAsync(int loadoutWeaponId, string userId);
        Task<Loadout?> GetLoadoutWithWeaponsAsync(int id);
    }

    public class LoadoutService : ILoadoutService
    {
        private readonly ApplicationDbContext _context;

        public LoadoutService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Loadout>> GetUserLoadoutsAsync(string userId)
        {
            return await _context.Loadouts
                .Where(l => l.UserId == userId)
                .Include(l => l.LoadoutWeapons)
                .ThenInclude(lw => lw.Weapon)
                .OrderByDescending(l => l.UpdatedAt)
                .ToListAsync();
        }

        public async Task<Loadout?> GetLoadoutByIdAsync(int id, string userId)
        {
            return await _context.Loadouts
                .Include(l => l.LoadoutWeapons)
                    .ThenInclude(lw => lw.Weapon)
                .FirstOrDefaultAsync(l => l.Id == id && l.UserId == userId);
        }        public async Task<Loadout> CreateLoadoutAsync(Loadout loadout)
        {
            loadout.CreatedAt = DateTime.UtcNow;
            loadout.UpdatedAt = DateTime.UtcNow;
            _context.Loadouts.Add(loadout);
            await _context.SaveChangesAsync();
            return loadout;
        }

        public async Task<bool> UpdateLoadoutAsync(Loadout loadout, string userId)
        {
            var existingLoadout = await _context.Loadouts.FindAsync(loadout.Id);
            if (existingLoadout == null || existingLoadout.UserId != userId)
                return false;

            existingLoadout.Name = loadout.Name;
            existingLoadout.Description = loadout.Description;
            existingLoadout.UpdatedAt = DateTime.UtcNow;

            _context.Loadouts.Update(existingLoadout);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteLoadoutAsync(int id, string userId)
        {
            var loadout = await _context.Loadouts.FindAsync(id);
            if (loadout == null || loadout.UserId != userId)
                return false;

            _context.Loadouts.Remove(loadout);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> AddWeaponToLoadoutAsync(int loadoutId, int weaponId, EquipSlot slot, string userId)
        {
            var loadout = await _context.Loadouts
                .Where(l => l.Id == loadoutId && l.UserId == userId)
                .FirstOrDefaultAsync();

            if (loadout == null)
                return false;

            var weapon = await _context.Weapons.FindAsync(weaponId);
            if (weapon == null)
                return false;

            // Check if slot already has a weapon
            var existingWeaponInSlot = await _context.LoadoutWeapons
                .FirstOrDefaultAsync(lw => lw.LoadoutId == loadoutId && lw.Slot == slot);

            if (existingWeaponInSlot != null)
                _context.LoadoutWeapons.Remove(existingWeaponInSlot);

            var loadoutWeapon = new LoadoutWeapon
            {
                LoadoutId = loadoutId,
                WeaponId = weaponId,
                Slot = slot
            };

            _context.LoadoutWeapons.Add(loadoutWeapon);
            loadout.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RemoveWeaponFromLoadoutAsync(int loadoutWeaponId, string userId)
        {
            var loadoutWeapon = await _context.LoadoutWeapons
                .Include(lw => lw.Loadout)
                .FirstOrDefaultAsync(lw => lw.Id == loadoutWeaponId);

            if (loadoutWeapon == null || loadoutWeapon.Loadout == null || loadoutWeapon.Loadout.UserId != userId)
                return false;

            _context.LoadoutWeapons.Remove(loadoutWeapon);
            loadoutWeapon.Loadout.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<Loadout?> GetLoadoutWithWeaponsAsync(int id)
        {
            return await _context.Loadouts
                .Include(l => l.LoadoutWeapons)
                .ThenInclude(lw => lw.Weapon)
                .FirstOrDefaultAsync(l => l.Id == id);
        }
    }
}

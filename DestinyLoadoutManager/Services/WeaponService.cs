using DestinyLoadoutManager.Data;
using DestinyLoadoutManager.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DestinyLoadoutManager.Services
{
    public interface IWeaponService
    {
        Task<List<Weapon>> GetAllWeaponsAsync();
        Task<Weapon?> GetWeaponByIdAsync(int id);
        Task<List<Weapon>> GetWeaponsBySlotAsync(EquipSlot slot);
        Task<List<Weapon>> GetWeaponsByElementAsync(ElementType element);
        Task<List<Weapon>> GetWeaponsByTypeAsync(WeaponType type);
        Task<Weapon> CreateWeaponAsync(Weapon weapon);
        Task<bool> UpdateWeaponAsync(Weapon weapon);
        Task<bool> DeleteWeaponAsync(int id);
    }

    public class WeaponService : IWeaponService
    {
        private readonly ApplicationDbContext _context;

        public WeaponService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Weapon>> GetAllWeaponsAsync()
        {
            return await _context.Weapons.OrderBy(w => w.Name).ToListAsync();
        }

        public async Task<Weapon?> GetWeaponByIdAsync(int id)
        {
            return await _context.Weapons.FindAsync(id);
        }

        public async Task<List<Weapon>> GetWeaponsBySlotAsync(EquipSlot slot)
        {
            return await _context.Weapons
                .Where(w => w.Slot == slot)
                .OrderBy(w => w.Name)
                .ToListAsync();
        }

        public async Task<List<Weapon>> GetWeaponsByElementAsync(ElementType element)
        {
            return await _context.Weapons
                .Where(w => w.Element == element)
                .OrderBy(w => w.Name)
                .ToListAsync();
        }

        public async Task<List<Weapon>> GetWeaponsByTypeAsync(WeaponType type)
        {
            return await _context.Weapons
                .Where(w => w.Type == type)
                .OrderBy(w => w.Name)
                .ToListAsync();
        }

        public async Task<Weapon> CreateWeaponAsync(Weapon weapon)
        {
            _context.Weapons.Add(weapon);
            await _context.SaveChangesAsync();
            return weapon;
        }

        public async Task<bool> UpdateWeaponAsync(Weapon weapon)
        {
            _context.Weapons.Update(weapon);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteWeaponAsync(int id)
        {
            var weapon = await _context.Weapons.FindAsync(id);
            if (weapon == null)
                return false;

            _context.Weapons.Remove(weapon);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}

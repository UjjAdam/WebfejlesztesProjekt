using DestinyLoadoutManager.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace DestinyLoadoutManager.Data
{
    public class DbInitializer
    {
        public static void Initialize(ApplicationDbContext context)
        {
            // Migrate any pending migrations
            context.Database.Migrate();

            // Seed Weapons
            if (!context.Weapons.Any())
            {
                var weapons = new List<Weapon>
                {
                    // Primary Slot - Long Range (Anti-Barrier)
                    new Weapon { Name = "Hung Jury", Type = WeaponType.ScoutRifle, Element = ElementType.Solar, Slot = EquipSlot.Primary, AmmoType = AmmoType.Primary },
                    new Weapon { Name = "Polaris Lance", Type = WeaponType.ScoutRifle, Element = ElementType.Solar, Slot = EquipSlot.Primary, AmmoType = AmmoType.Primary },
                    new Weapon { Name = "Nameless Midnight", Type = WeaponType.ScoutRifle, Element = ElementType.Arc, Slot = EquipSlot.Primary, AmmoType = AmmoType.Primary },
                    new Weapon { Name = "Twilight Oath", Type = WeaponType.SniperRifle, Element = ElementType.Void, Slot = EquipSlot.Special, AmmoType = AmmoType.Special },
                    new Weapon { Name = "Supremacy", Type = WeaponType.SniperRifle, Element = ElementType.Arc, Slot = EquipSlot.Special, AmmoType = AmmoType.Special },
                    new Weapon { Name = "Frozen Orbit", Type = WeaponType.SniperRifle, Element = ElementType.Void, Slot = EquipSlot.Special, AmmoType = AmmoType.Special },
                    new Weapon { Name = "Sleeper Simulant", Type = WeaponType.LinearFusionRifle, Element = ElementType.Solar, Slot = EquipSlot.Heavy, AmmoType = AmmoType.Heavy },
                    new Weapon { Name = "Arbalest", Type = WeaponType.LinearFusionRifle, Element = ElementType.Kinetic, Slot = EquipSlot.Special, AmmoType = AmmoType.Special },
                    new Weapon { Name = "Vigilance Wing", Type = WeaponType.PulseRifle, Element = ElementType.Kinetic, Slot = EquipSlot.Primary, AmmoType = AmmoType.Primary },
                    new Weapon { Name = "Outbreak Perfected", Type = WeaponType.PulseRifle, Element = ElementType.Kinetic, Slot = EquipSlot.Primary, AmmoType = AmmoType.Primary },

                    // Primary Slot - Continuous Fire (Overload)
                    new Weapon { Name = "Suros Regime", Type = WeaponType.AutoRifle, Element = ElementType.Solar, Slot = EquipSlot.Primary, AmmoType = AmmoType.Primary },
                    new Weapon { Name = "Hard Light", Type = WeaponType.AutoRifle, Element = ElementType.Kinetic, Slot = EquipSlot.Primary, AmmoType = AmmoType.Primary },
                    new Weapon { Name = "Monte Carlo", Type = WeaponType.AutoRifle, Element = ElementType.Arc, Slot = EquipSlot.Primary, AmmoType = AmmoType.Primary },
                    new Weapon { Name = "Huckleberry", Type = WeaponType.SubmachineGun, Element = ElementType.Kinetic, Slot = EquipSlot.Primary, AmmoType = AmmoType.Primary },
                    new Weapon { Name = "Riskrunner", Type = WeaponType.SubmachineGun, Element = ElementType.Arc, Slot = EquipSlot.Primary, AmmoType = AmmoType.Primary },
                    new Weapon { Name = "The Hive", Type = WeaponType.SubmachineGun, Element = ElementType.Void, Slot = EquipSlot.Primary, AmmoType = AmmoType.Primary },
                    new Weapon { Name = "Thunderlord", Type = WeaponType.MachineGun, Element = ElementType.Arc, Slot = EquipSlot.Heavy, AmmoType = AmmoType.Heavy },
                    new Weapon { Name = "Tyrant's Surge", Type = WeaponType.MachineGun, Element = ElementType.Solar, Slot = EquipSlot.Heavy, AmmoType = AmmoType.Heavy },

                    // Special/Heavy - Burst (Unstoppable)
                    new Weapon { Name = "Telesto", Type = WeaponType.FusionRifle, Element = ElementType.Void, Slot = EquipSlot.Special, AmmoType = AmmoType.Special },
                    new Weapon { Name = "Merciless", Type = WeaponType.FusionRifle, Element = ElementType.Arc, Slot = EquipSlot.Special, AmmoType = AmmoType.Special },
                    new Weapon { Name = "Jotunn", Type = WeaponType.FusionRifle, Element = ElementType.Solar, Slot = EquipSlot.Special, AmmoType = AmmoType.Special },
                    new Weapon { Name = "Gjallarhorn", Type = WeaponType.RocketLauncher, Element = ElementType.Solar, Slot = EquipSlot.Heavy, AmmoType = AmmoType.Heavy },
                    new Weapon { Name = "Wardcliff Coil", Type = WeaponType.RocketLauncher, Element = ElementType.Void, Slot = EquipSlot.Heavy, AmmoType = AmmoType.Heavy },
                    new Weapon { Name = "Tractor Cannon", Type = WeaponType.RocketLauncher, Element = ElementType.Void, Slot = EquipSlot.Heavy, AmmoType = AmmoType.Heavy },
                    new Weapon { Name = "The Jade Rabbit", Type = WeaponType.ScoutRifle, Element = ElementType.Solar, Slot = EquipSlot.Primary, AmmoType = AmmoType.Primary },
                    new Weapon { Name = "Ace of Spades", Type = WeaponType.HandCannon, Element = ElementType.Solar, Slot = EquipSlot.Primary, AmmoType = AmmoType.Primary },
                    new Weapon { Name = "Malfeasance", Type = WeaponType.HandCannon, Element = ElementType.Void, Slot = EquipSlot.Primary, AmmoType = AmmoType.Primary },

                    // Additional variety
                    new Weapon { Name = "Legend of Acrius", Type = WeaponType.Shotgun, Element = ElementType.Arc, Slot = EquipSlot.Heavy, AmmoType = AmmoType.Heavy },
                    new Weapon { Name = "Duality", Type = WeaponType.Shotgun, Element = ElementType.Kinetic, Slot = EquipSlot.Special, AmmoType = AmmoType.Special },
                    new Weapon { Name = "Thorn", Type = WeaponType.HandCannon, Element = ElementType.Solar, Slot = EquipSlot.Primary, AmmoType = AmmoType.Primary },
                    new Weapon { Name = "The Last Word", Type = WeaponType.HandCannon, Element = ElementType.Solar, Slot = EquipSlot.Primary, AmmoType = AmmoType.Primary },
                    new Weapon { Name = "Rat King", Type = WeaponType.SubmachineGun, Element = ElementType.Kinetic, Slot = EquipSlot.Primary, AmmoType = AmmoType.Primary },
                    new Weapon { Name = "Skyburner's Oath", Type = WeaponType.ScoutRifle, Element = ElementType.Solar, Slot = EquipSlot.Primary, AmmoType = AmmoType.Primary },
                };

                context.Weapons.AddRange(weapons);
                context.SaveChanges();
            }

            // Seed Champions
            if (!context.Champions.Any())
            {
                var champions = new List<Champion>
                {
                    new Champion 
                    { 
                        Name = "Anti-Barrier",
                        Description = "A barrier-creating champion. Use long-range weapons." 
                    },
                    new Champion 
                    { 
                        Name = "Overload",
                        Description = "A champion that gains extra health at certain points. Use automatic weapons." 
                    },
                    new Champion 
                    { 
                        Name = "Unstoppable",
                        Description = "A powerful champion. Use burst-damage weapons." 
                    }
                };

                context.Champions.AddRange(champions);
                context.SaveChanges();
            }

            // Seed ChampionWeaponTypes
            if (!context.ChampionWeaponTypes.Any())
            {
                var antiBarrier = context.Champions.FirstOrDefault(c => c.Name == "Anti-Barrier");
                var overload = context.Champions.FirstOrDefault(c => c.Name == "Overload");
                var unstoppable = context.Champions.FirstOrDefault(c => c.Name == "Unstoppable");

                var championWeaponTypes = new List<ChampionWeaponType>
                {
                    // Anti-Barrier: Long range
                    new ChampionWeaponType { ChampionId = antiBarrier.Id, WeaponType = WeaponType.SniperRifle },
                    new ChampionWeaponType { ChampionId = antiBarrier.Id, WeaponType = WeaponType.ScoutRifle },
                    new ChampionWeaponType { ChampionId = antiBarrier.Id, WeaponType = WeaponType.PulseRifle },
                    new ChampionWeaponType { ChampionId = antiBarrier.Id, WeaponType = WeaponType.LinearFusionRifle },

                    // Overload: Continuous fire
                    new ChampionWeaponType { ChampionId = overload.Id, WeaponType = WeaponType.AutoRifle },
                    new ChampionWeaponType { ChampionId = overload.Id, WeaponType = WeaponType.SubmachineGun },
                    new ChampionWeaponType { ChampionId = overload.Id, WeaponType = WeaponType.MachineGun },

                    // Unstoppable: Burst damage
                    new ChampionWeaponType { ChampionId = unstoppable.Id, WeaponType = WeaponType.FusionRifle },
                    new ChampionWeaponType { ChampionId = unstoppable.Id, WeaponType = WeaponType.RocketLauncher },
                    new ChampionWeaponType { ChampionId = unstoppable.Id, WeaponType = WeaponType.GrenadeGauncher },
                };

                context.ChampionWeaponTypes.AddRange(championWeaponTypes);
                context.SaveChanges();
            }

            // Seed Surges
            if (!context.Surges.Any())
            {
                var surges = new List<Surge>
                {
                    new Surge { Name = "Arc Surge", ElementType = ElementType.Arc },
                    new Surge { Name = "Void Surge", ElementType = ElementType.Void },
                    new Surge { Name = "Solar Surge", ElementType = ElementType.Solar },
                    new Surge { Name = "Kinetic Surge", ElementType = ElementType.Kinetic },
                };

                context.Surges.AddRange(surges);
                context.SaveChanges();
            }
        }
    }
}

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DestinyLoadoutManager.Models
{
    public class Weapon
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [Required]
        public WeaponType Type { get; set; }

        [Required]
        public ElementType Element { get; set; }

        [Required]
        public EquipSlot Slot { get; set; }

        [Required]
        public AmmoType AmmoType { get; set; }

        public virtual ICollection<LoadoutWeapon> LoadoutWeapons { get; set; } = new List<LoadoutWeapon>();
    }

    public enum WeaponType
    {
        PulseRifle,
        ScoutRifle,
        SniperRifle,
        AutoRifle,
        SubmachineGun,
        MachineGun,
        FusionRifle,
        RocketLauncher,
        GrenadeGauncher,
        HandCannon,
        Shotgun,
        LinearFusionRifle,
        Bow,
        Sword
    }

    public enum ElementType
    {
        Arc,
        Void,
        Solar,
        Kinetic
    }

    public enum EquipSlot
    {
        Primary,
        Special,
        Heavy
    }

    public enum AmmoType
    {
        Primary,
        Special,
        Heavy
    }
}

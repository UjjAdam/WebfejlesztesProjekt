using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DestinyLoadoutManager.Models
{
    public class LoadoutWeapon
    {
        public int Id { get; set; }

        public int LoadoutId { get; set; }

        [ForeignKey(nameof(LoadoutId))]
        public virtual Loadout Loadout { get; set; }

        public int WeaponId { get; set; }

        [ForeignKey(nameof(WeaponId))]
        public virtual Weapon Weapon { get; set; }

        [Required]
        public EquipSlot Slot { get; set; }
    }
}

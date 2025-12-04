using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DestinyLoadoutManager.Models
{
    public class Champion
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; } = null!; // Anti-Barrier, Overload, Unstoppable

        public string? Description { get; set; }

        public virtual ICollection<ChampionWeaponType> ChampionWeaponTypes { get; set; } = new List<ChampionWeaponType>();
    }

    public class ChampionWeaponType
    {
        public int Id { get; set; }

        public int ChampionId { get; set; }
        public virtual Champion Champion { get; set; } = null!;

        public WeaponType WeaponType { get; set; }
    }
}

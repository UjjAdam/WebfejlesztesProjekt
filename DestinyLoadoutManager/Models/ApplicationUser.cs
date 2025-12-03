using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace DestinyLoadoutManager.Models
{
    public class ApplicationUser : IdentityUser
    {
        public virtual ICollection<Loadout> Loadouts { get; set; } = new List<Loadout>();
    }
}

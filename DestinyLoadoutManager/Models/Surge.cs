using System.ComponentModel.DataAnnotations;

namespace DestinyLoadoutManager.Models
{
    public class Surge
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; } = string.Empty;

        public ElementType ElementType { get; set; }
    }
}

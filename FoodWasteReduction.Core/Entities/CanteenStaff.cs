using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FoodWasteReduction.Core.Entities
{
    public class CanteenStaff
    {
        [Key]
        public string Id { get; set; } = string.Empty; // FK to AspNetUsers

        [Required]
        public string PersonnelNumber { get; set; } = string.Empty;

        [Required]
        [ForeignKey("Canteen")]
        public int CanteenId { get; set; }

        public virtual Canteen? Canteen { get; set; }
    }
}

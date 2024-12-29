using System.ComponentModel.DataAnnotations;
using FoodWasteReduction.Core.Enums;

namespace FoodWasteReduction.Core.Entities
{
    public class CanteenStaff
    {
        [Key]
        public string Id { get; set; } = string.Empty; // FK to AspNetUsers

        [Required]
        public string PersonnelNumber { get; set; } = string.Empty;

        [Required]
        public Location Location { get; set; }
    }
}

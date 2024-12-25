using System.ComponentModel.DataAnnotations;
using FoodWasteReduction.Core.Enums;

namespace FoodWasteReduction.Core.Entities
{
    public class CanteenStaff : ApplicationUser
    {
        [Required]
        public string PersonnelNumber { get; set; } = string.Empty;

        [Required]
        public Location Location { get; set; }
    }
}

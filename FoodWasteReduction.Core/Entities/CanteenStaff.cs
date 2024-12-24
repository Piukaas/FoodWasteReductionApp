using System.ComponentModel.DataAnnotations;

namespace FoodWasteReduction.Core.Entities
{
    public class CanteenStaff : ApplicationUser
    {
        [Required]
        public string PersonnelNumber { get; set; } = string.Empty;

        [Required]
        public string Location { get; set; } = string.Empty;
    }
}
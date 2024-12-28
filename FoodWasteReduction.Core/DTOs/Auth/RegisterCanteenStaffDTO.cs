using System.ComponentModel.DataAnnotations;
using FoodWasteReduction.Core.Enums;

namespace FoodWasteReduction.Core.DTOs.Auth
{
    public class RegisterCanteenStaffDTO
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string Name { get; set; } = string.Empty;

        [Required]
        public string PersonnelNumber { get; set; } = string.Empty;

        [Required]
        public Location Location { get; set; }

        [Required]
        [MinLength(8)]
        public string Password { get; set; } = string.Empty;
    }
}

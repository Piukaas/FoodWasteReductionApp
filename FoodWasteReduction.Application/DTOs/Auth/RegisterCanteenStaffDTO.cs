using System.ComponentModel.DataAnnotations;

namespace FoodWasteReduction.Application.DTOs.Auth
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
        public int CanteenId { get; set; }

        [Required]
        [MinLength(8)]
        public string Password { get; set; } = string.Empty;
    }
}

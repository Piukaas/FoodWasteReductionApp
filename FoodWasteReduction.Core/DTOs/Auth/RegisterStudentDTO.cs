using System.ComponentModel.DataAnnotations;
using FoodWasteReduction.Core.Validation;

namespace FoodWasteReduction.Core.DTOs.Auth
{
    public class RegisterStudentDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string Name { get; set; } = string.Empty;

        [Required]
        public string StudentNumber { get; set; } = string.Empty;

        [Required]
        [MinimumAge(16)]
        public DateTime DateOfBirth { get; set; }

        [Required]
        public string StudyCity { get; set; } = string.Empty;

        [Required]
        [MinLength(8)]
        public string Password { get; set; } = string.Empty;
    }
}
using System.ComponentModel.DataAnnotations;
using FoodWasteReduction.Core.Enums;
using FoodWasteReduction.Core.Validation;

namespace FoodWasteReduction.Core.Entities
{
    public class Student
    {
        [Key]
        public string Id { get; set; } = string.Empty; // FK to AspNetUsers

        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;

        [Required]
        [MinimumAge(16)]
        public DateTime DateOfBirth { get; set; }

        [Required]
        public string StudentNumber { get; set; } = string.Empty;

        [Required]
        public City StudyCity { get; set; }
    }
}

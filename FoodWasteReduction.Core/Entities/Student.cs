using System.ComponentModel.DataAnnotations;
using FoodWasteReduction.Core.Enums;
using FoodWasteReduction.Core.Validation;

namespace FoodWasteReduction.Core.Entities
{
    public class Student : ApplicationUser
    {
        [Required]
        [MinimumAge(16)]
        public DateTime DateOfBirth { get; set; }

        [Required]
        public string StudentNumber { get; set; } = string.Empty;

        [Required]
        public City StudyCity { get; set; }
    }
}

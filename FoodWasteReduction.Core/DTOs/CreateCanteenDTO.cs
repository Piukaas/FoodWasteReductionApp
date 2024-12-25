using System.ComponentModel.DataAnnotations;
using FoodWasteReduction.Core.Enums;

namespace FoodWasteReduction.Core.DTOs
{
    public class CreateCanteenDTO
    {
        [Required]
        public City City { get; set; }

        [Required]
        public Location Location { get; set; }

        public bool ServesWarmMeals { get; set; } = false;
    }
}

using System.ComponentModel.DataAnnotations;
using FoodWasteReduction.Core.Enums;
using FoodWasteReduction.Core.Validations;

namespace FoodWasteReduction.Core.DTOs
{
    public class CreatePackageDTO
    {
        [Required]
        public string Name { get; set; } = string.Empty;

        [Required]
        public City City { get; set; }

        [Required]
        public int CanteenId { get; set; }

        [Required]
        public MealType Type { get; set; }

        [Required]
        [FutureDate]
        public DateTime PickupTime { get; set; }

        [Required]
        public DateTime ExpiryTime { get; set; }

        [Required]
        [Range(0, double.MaxValue)]
        public decimal Price { get; set; }

        [Required]
        public List<int> ProductIds { get; set; } = [];
    }
}

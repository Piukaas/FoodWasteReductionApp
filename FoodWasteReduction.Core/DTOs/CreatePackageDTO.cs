using System.ComponentModel.DataAnnotations;
using FoodWasteReduction.Core.Enums;

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
        public DateTime PickupTime { get; set; }

        [Required]
        public DateTime ExpiryTime { get; set; }

        [Required]
        [Range(0, double.MaxValue)]
        public decimal Price { get; set; }

        public bool Is18Plus { get; set; }

        public List<int> ProductIds { get; set; } = [];
    }
}

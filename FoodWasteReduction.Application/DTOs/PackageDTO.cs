using FoodWasteReduction.Core.Entities;
using FoodWasteReduction.Core.Enums;

namespace FoodWasteReduction.Application.DTOs
{
    public class PackageDTO(Package package)
    {
        public int Id { get; set; } = package.Id;
        public string Name { get; set; } = package.Name;
        public City City { get; set; } = package.City;
        public int CanteenId { get; set; } = package.CanteenId;
        public MealType Type { get; set; } = package.Type;
        public DateTime PickupTime { get; set; } = package.PickupTime;
        public DateTime ExpiryTime { get; set; } = package.ExpiryTime;
        public decimal Price { get; set; } = package.Price;
        public bool Is18Plus { get; set; } = package.Is18Plus;
        public string? ReservedById { get; set; } = package.ReservedById;
        public List<ProductDTO> Products { get; set; } =
            package.Products?.Select(p => new ProductDTO(p)).ToList() ?? [];
    }
}

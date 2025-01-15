using FoodWasteReduction.Core.Entities;

namespace FoodWasteReduction.Application.DTOs
{
    public class ProductDTO(Product product)
    {
        public int Id { get; set; } = product.Id;
        public string Name { get; set; } = product.Name;
        public bool ContainsAlcohol { get; set; } = product.ContainsAlcohol;
        public string? ImageUrl { get; set; } = product.ImageUrl;
    }
}

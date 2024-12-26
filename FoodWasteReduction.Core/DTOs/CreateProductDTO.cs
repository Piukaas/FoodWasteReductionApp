using System.ComponentModel.DataAnnotations;

namespace FoodWasteReduction.Core.DTOs
{
    public class CreateProductDTO
    {
        [Required]
        public string Name { get; set; } = string.Empty;

        public bool ContainsAlcohol { get; set; } = false;

        public string? ImageUrl { get; set; }
    }
}

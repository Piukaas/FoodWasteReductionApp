using System.Text.Json.Serialization;

namespace FoodWasteReduction.Application.DTOs.Json
{
    public class JsonProductDTO
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("containsAlcohol")]
        public bool ContainsAlcohol { get; set; }

        [JsonPropertyName("imageUrl")]
        public string? ImageUrl { get; set; }
    }
}

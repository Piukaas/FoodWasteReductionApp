using System.Text.Json.Serialization;
using FoodWasteReduction.Core.Enums;

namespace FoodWasteReduction.Application.DTOs.Json
{
    public class JsonPackageDTO
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("city")]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public City City { get; set; }

        [JsonPropertyName("type")]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public MealType Type { get; set; }

        [JsonPropertyName("pickupTime")]
        public DateTime PickupTime { get; set; }

        [JsonPropertyName("expiryTime")]
        public DateTime ExpiryTime { get; set; }

        [JsonPropertyName("price")]
        public decimal Price { get; set; }

        [JsonPropertyName("is18Plus")]
        public bool Is18Plus { get; set; }

        [JsonPropertyName("products")]
        public List<JsonProductDTO> Products { get; set; } = [];

        [JsonPropertyName("canteen")]
        public JsonCanteenDTO? Canteen { get; set; }

        [JsonPropertyName("reservedById")]
        public string? ReservedById { get; set; }

        [JsonPropertyName("reservedBy")]
        public JsonStudentDTO? ReservedBy { get; set; }
    }
}

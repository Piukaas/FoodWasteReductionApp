using System.Text.Json.Serialization;
using FoodWasteReduction.Core.Enums;

namespace FoodWasteReduction.Application.DTOs.Json
{
    public class JsonCanteenDTO
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("city")]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public City City { get; set; }

        [JsonPropertyName("location")]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public Location Location { get; set; }

        [JsonPropertyName("servesWarmMeals")]
        public bool ServesWarmMeals { get; set; }
    }
}

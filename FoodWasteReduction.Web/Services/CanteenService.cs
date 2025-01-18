using System.Text.Json;
using System.Text.Json.Serialization;
using FoodWasteReduction.Application.DTOs.Json;
using FoodWasteReduction.Web.Services.Interfaces;

namespace FoodWasteReduction.Web.Services
{
    public class CanteenService(IHttpClientFactory httpClientFactory) : ICanteenService
    {
        private readonly HttpClient _httpClient = httpClientFactory.CreateClient("API");
        private readonly JsonSerializerOptions _jsonOptions = new()
        {
            PropertyNameCaseInsensitive = true,
            Converters = { new JsonStringEnumConverter() },
        };

        public async Task<IEnumerable<JsonCanteenDTO>> GetCanteens()
        {
            var response = await _httpClient.GetAsync("api/Canteens");
            if (!response.IsSuccessStatusCode)
                return [];

            var content = await response.Content.ReadAsStringAsync();

            var canteens = JsonSerializer.Deserialize<IEnumerable<JsonCanteenDTO>>(
                content,
                _jsonOptions
            );

            return canteens ?? [];
        }
    }
}

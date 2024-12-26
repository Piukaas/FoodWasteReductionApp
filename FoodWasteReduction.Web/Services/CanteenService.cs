using FoodWasteReduction.Core.Entities;
using FoodWasteReduction.Web.Services.Interfaces;

namespace FoodWasteReduction.Web.Services
{
    public class CanteenService(IHttpClientFactory httpClientFactory) : ICanteenService
    {
        private readonly HttpClient _httpClient = httpClientFactory.CreateClient("API");

        public async Task<IEnumerable<Canteen>> GetCanteens()
        {
            var response = await _httpClient.GetAsync("api/Canteens");
            if (!response.IsSuccessStatusCode)
                return [];

            return await response.Content.ReadFromJsonAsync<IEnumerable<Canteen>>() ?? [];
        }
    }
}

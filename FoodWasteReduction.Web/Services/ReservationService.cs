using System.Net.Http.Headers;
using FoodWasteReduction.Application.DTOs;
using FoodWasteReduction.Web.Services.Interfaces;

namespace FoodWasteReduction.Web.Services
{
    public class ReservationService(
        IHttpClientFactory clientFactory,
        IAuthGuardService authGuardService
    ) : IReservationService
    {
        private readonly HttpClient _httpClient = clientFactory.CreateClient("API");
        private readonly IAuthGuardService _authGuardservice = authGuardService;

        public async Task<(bool success, string? errorMessage)> ReservePackage(
            int packageId,
            string userId
        )
        {
            try
            {
                var token = _authGuardservice.GetToken();
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                    "Bearer",
                    token
                );

                var response = await _httpClient.PostAsJsonAsync(
                    "api/Reservation",
                    new { packageId, userId }
                );

                if (response.IsSuccessStatusCode)
                    return (true, null);

                var error = await response.Content.ReadFromJsonAsync<ErrorResponse>();
                return (false, error?.Message ?? "Failed to reserve package");
            }
            catch (Exception)
            {
                return (false, "Er is iets misgegaan bij het reserveren van het pakket");
            }
        }
    }
}

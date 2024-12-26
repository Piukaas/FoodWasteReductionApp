using System.Text.Json;
using FoodWasteReduction.Core.Enums;
using FoodWasteReduction.Web.Models.Auth;
using FoodWasteReduction.Web.Services.Interfaces;

namespace FoodWasteReduction.Web.Services
{
    public class AuthService(IHttpClientFactory httpClientFactory) : IAuthService
    {
        private readonly HttpClient _httpClient = httpClientFactory.CreateClient("API");

        public async Task<(bool success, string token, object? userData)> Login(
            LoginViewModel model
        )
        {
            var response = await _httpClient.PostAsJsonAsync("api/Auth/login", model);
            if (!response.IsSuccessStatusCode)
                return (false, string.Empty, null);

            var content = await response.Content.ReadFromJsonAsync<
                Dictionary<string, JsonElement>
            >();
            if (content == null)
                return (false, string.Empty, null);

            var responseData = content["responseData"];
            var additionalData = content["additionalData"];

            var token = responseData.GetProperty("token").GetString() ?? string.Empty;
            var roles = responseData
                .GetProperty("roles")
                .EnumerateArray()
                .Select(r => r.GetString())
                .Where(r => r != null)
                .ToList()!;

            var userData = new
            {
                Id = responseData.GetProperty("id").GetString() ?? string.Empty,
                Email = responseData.GetProperty("email").GetString() ?? string.Empty,
                Name = responseData.GetProperty("name").GetString() ?? string.Empty,
                Roles = roles,
                StudyCity = additionalData.TryGetProperty("StudyCity", out var city)
                    ? (City?)city.GetInt32()
                    : null,
                Location = additionalData.TryGetProperty("Location", out var location)
                    ? (Location?)location.GetInt32()
                    : null,
                DateOfBirth = additionalData.TryGetProperty("DateOfBirth", out var dob)
                    ? (DateTime?)dob.GetDateTime()
                    : null,
            };

            return (true, token, userData);
        }

        public async Task<bool> RegisterStudent(RegisterStudentViewModel model)
        {
            var response = await _httpClient.PostAsJsonAsync("api/Auth/register/student", model);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> RegisterCanteenStaff(RegisterCanteenStaffViewModel model)
        {
            var response = await _httpClient.PostAsJsonAsync(
                "api/Auth/register/canteenstaff",
                model
            );
            return response.IsSuccessStatusCode;
        }
    }
}

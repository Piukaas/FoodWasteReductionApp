using System.Text.Json;
using FoodWasteReduction.Web.Models.Auth;

public class AuthService : IAuthService
{
    private readonly HttpClient _httpClient;

    public AuthService(IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient("API");
    }

    public async Task<(bool success, string token, object? userData)> Login(LoginViewModel model)
    {
        var response = await _httpClient.PostAsJsonAsync("api/Auth/login", model);
        if (!response.IsSuccessStatusCode)
            return (false, string.Empty, null);

        var responseData = await response.Content.ReadFromJsonAsync<Dictionary<string, object>>();
        if (responseData == null || !responseData.ContainsKey("token"))
            return (false, string.Empty, null);

        var token = responseData["token"]?.ToString() ?? string.Empty;
        var roles =
            responseData["roles"] is JsonElement rolesElement
            && rolesElement.ValueKind == JsonValueKind.Array
                ? rolesElement
                    .EnumerateArray()
                    .Select(static r => r.GetString())
                    .Where(static r => r != null)
                    .ToList()!
                : new List<string>();

        var userData = new
        {
            Email = responseData["email"]?.ToString() ?? string.Empty,
            Name = responseData["name"]?.ToString() ?? string.Empty,
            Roles = roles,
            DateOfBirth = responseData["dateOfBirth"]?.ToString() ?? string.Empty,
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
        var response = await _httpClient.PostAsJsonAsync("api/Auth/register/canteenstaff", model);
        return response.IsSuccessStatusCode;
    }
}

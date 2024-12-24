using FoodWasteReduction.Web.Models.Auth;

public class AuthService : IAuthService
{
    private readonly HttpClient _httpClient;

    public AuthService(IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient("API");
    }

    public async Task<(bool success, string token)> Login(LoginViewModel model)
    {
        var response = await _httpClient.PostAsJsonAsync("api/Auth/login", model);
        if (!response.IsSuccessStatusCode)
            return (false, string.Empty);

        var token = await response.Content.ReadAsStringAsync();
        return (true, token);
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

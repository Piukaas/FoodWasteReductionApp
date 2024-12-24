using FoodWasteReduction.Web.Models.Auth;

public class AuthService : IAuthService
{
    private readonly HttpClient _httpClient;

    public AuthService(IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient("API");
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

    public async Task<bool> Login(LoginViewModel model)
    {
        var response = await _httpClient.PostAsJsonAsync("api/Auth/login", model);
        return response.IsSuccessStatusCode;
    }
}

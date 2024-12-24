using System.IdentityModel.Tokens.Jwt;

public class AuthGuardService : IAuthGuardService
{
    private string? _token;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public AuthGuardService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
        _token = _httpContextAccessor.HttpContext?.Session.GetString("JWTToken");
    }

    public bool IsAuthenticated => !string.IsNullOrEmpty(_token);

    public bool HasRole(string role)
    {
        if (string.IsNullOrEmpty(_token))
            return false;

        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadJwtToken(_token);

        var roles = jwtToken.Claims.Where(c => c.Type == "role").Select(c => c.Value);
        return roles.Contains(role);
    }

    public void SetToken(string token)
    {
        _token = token;
        _httpContextAccessor.HttpContext?.Session.SetString("JWTToken", token);
    }

    public void ClearToken()
    {
        _token = null;
        _httpContextAccessor.HttpContext?.Session.Remove("JWTToken");
    }

    public string? GetToken() => _token;
}

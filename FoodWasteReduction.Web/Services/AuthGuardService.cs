using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

public class AuthGuardService : IAuthGuardService
{
    private string? _token;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public AuthGuardService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
        _token = _httpContextAccessor.HttpContext?.Session.GetString("JWTToken");
    }

    public bool IsAuthenticated => !string.IsNullOrEmpty(GetToken());

    public bool HasRole(string role)
    {
        try
        {
            var token = GetToken();
            if (string.IsNullOrEmpty(token))
                return false;

            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);

            var roles = jwtToken.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value);

            return roles.Contains(role);
        }
        catch
        {
            return false;
        }
    }

    public void SetToken(string token)
    {
        if (string.IsNullOrEmpty(token))
            return;

        _token = token;
        if (_httpContextAccessor.HttpContext != null)
        {
            _httpContextAccessor.HttpContext.Session.SetString("JWTToken", token);
        }
    }

    public void ClearToken()
    {
        _token = null;
        if (_httpContextAccessor.HttpContext != null)
        {
            _httpContextAccessor.HttpContext.Session.Remove("JWTToken");
        }
    }

    public string? GetToken()
    {
        if (string.IsNullOrEmpty(_token))
        {
            _token = _httpContextAccessor.HttpContext?.Session.GetString("JWTToken");
        }
        return _token;
    }
}

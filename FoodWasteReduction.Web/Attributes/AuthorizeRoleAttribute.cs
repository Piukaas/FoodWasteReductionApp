using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class AuthorizeRoleAttribute : Attribute, IAuthorizationFilter
{
    private readonly string _role;

    public AuthorizeRoleAttribute(string role)
    {
        _role = role;
    }

    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var authGuardService =
            context.HttpContext.RequestServices.GetRequiredService<IAuthGuardService>();

        if (!authGuardService.IsAuthenticated)
        {
            context.Result = new RedirectToActionResult("Login", "Auth", null);
            return;
        }

        if (!authGuardService.HasRole(_role))
        {
            context.Result = new ForbidResult();
        }
    }
}

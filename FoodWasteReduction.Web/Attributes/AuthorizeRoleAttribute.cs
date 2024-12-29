using FoodWasteReduction.Web.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace FoodWasteReduction.Web.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class AuthorizeRoleAttribute(params string[] roles) : Attribute, IAuthorizationFilter
    {
        private readonly string[] _roles = roles;

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var authGuardService =
                context.HttpContext.RequestServices.GetRequiredService<IAuthGuardService>();

            if (!authGuardService.IsAuthenticated)
            {
                context.Result = new RedirectToActionResult("Login", "Auth", null);
                return;
            }

            if (!_roles.Any(authGuardService.HasRole))
            {
                context.Result = new ForbidResult();
            }
        }
    }
}

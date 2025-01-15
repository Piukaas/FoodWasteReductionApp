using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FoodWasteReduction.Tests.Controllers.Api
{
    public abstract class ApiControllerTestBase
    {
        protected ClaimsPrincipal User;

        protected ApiControllerTestBase()
        {
            User = new ClaimsPrincipal(new ClaimsIdentity());
        }

        protected void SetupController<T>(T controller)
            where T : ControllerBase
        {
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = User },
            };
        }

        protected void SetupUserRole<T>(string role, T controller)
            where T : ControllerBase
        {
            var claims = new List<Claim>
            {
                new(ClaimTypes.Role, role),
                new(ClaimTypes.NameIdentifier, "testUser"),
            };
            User = new ClaimsPrincipal(new ClaimsIdentity(claims, "Bearer"));
            SetupController(controller);
        }
    }
}

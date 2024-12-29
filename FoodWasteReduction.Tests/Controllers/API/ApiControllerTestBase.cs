using System.Security.Claims;
using FoodWasteReduction.Core.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace FoodWasteReduction.Tests.Controllers.Api
{
    public abstract class ApiControllerTestBase
    {
        protected Mock<UserManager<ApplicationUser>> UserManager;
        protected Mock<SignInManager<ApplicationUser>> SignInManager;
        protected ClaimsPrincipal User;

        protected ApiControllerTestBase()
        {
            UserManager = GetMockUserManager();
            SignInManager = GetMockSignInManager();
            User = new ClaimsPrincipal(new ClaimsIdentity());
        }

        private static Mock<SignInManager<ApplicationUser>> GetMockSignInManager()
        {
            var userManager = GetMockUserManager();
            var contextAccessor = new Mock<IHttpContextAccessor>();
            var claimsFactory = new Mock<IUserClaimsPrincipalFactory<ApplicationUser>>();

            return new Mock<SignInManager<ApplicationUser>>(
                userManager.Object,
                contextAccessor.Object,
                claimsFactory.Object,
                null,
                null,
                null,
                null
            );
        }

        private static Mock<UserManager<ApplicationUser>> GetMockUserManager()
        {
            var store = new Mock<IUserStore<ApplicationUser>>();
            var mgr = new Mock<UserManager<ApplicationUser>>(
                store.Object,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null
            );
            mgr.Object.UserValidators.Add(new UserValidator<ApplicationUser>());
            mgr.Object.PasswordValidators.Add(new PasswordValidator<ApplicationUser>());
            return mgr;
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

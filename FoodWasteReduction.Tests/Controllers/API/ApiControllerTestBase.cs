using System.Security.Claims;
using FoodWasteReduction.Core.Entities;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
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
            var options = new Mock<IOptions<IdentityOptions>>();
            var logger = new Mock<ILogger<SignInManager<ApplicationUser>>>();
            var schemes = new Mock<IAuthenticationSchemeProvider>();
            var confirmation = new Mock<IUserConfirmation<ApplicationUser>>();

            return new Mock<SignInManager<ApplicationUser>>(
                userManager.Object,
                contextAccessor.Object,
                claimsFactory.Object,
                options.Object,
                logger.Object,
                schemes.Object,
                confirmation.Object
            );
        }

        private static Mock<UserManager<ApplicationUser>> GetMockUserManager()
        {
            var store = new Mock<IUserStore<ApplicationUser>>();
            var options = new Mock<IOptions<IdentityOptions>>();
            var passwordHasher = new Mock<IPasswordHasher<ApplicationUser>>();
            var userValidators = new[] { new UserValidator<ApplicationUser>() };
            var passwordValidators = new[] { new PasswordValidator<ApplicationUser>() };
            var normalizer = new Mock<ILookupNormalizer>();
            var errorDescriber = new IdentityErrorDescriber();
            var services = new Mock<IServiceProvider>();
            var logger = new Mock<ILogger<UserManager<ApplicationUser>>>();

            options
                .Setup(o => o.Value)
                .Returns(
                    new IdentityOptions
                    {
                        Password = new PasswordOptions
                        {
                            RequireDigit = false,
                            RequiredLength = 8,
                            RequireLowercase = false,
                            RequireNonAlphanumeric = false,
                            RequireUppercase = false,
                        },
                    }
                );

            var mgr = new Mock<UserManager<ApplicationUser>>(
                store.Object,
                options.Object,
                passwordHasher.Object,
                userValidators,
                passwordValidators,
                normalizer.Object,
                errorDescriber,
                services.Object,
                logger.Object
            );

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

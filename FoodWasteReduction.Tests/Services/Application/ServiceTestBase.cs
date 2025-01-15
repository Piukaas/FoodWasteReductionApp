using FoodWasteReduction.Core.Entities;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;

namespace FoodWasteReduction.Tests.Services.Application
{
    public abstract class ServiceTestBase
    {
        protected Mock<UserManager<ApplicationUser>> UserManager;
        protected Mock<SignInManager<ApplicationUser>> SignInManager;
        protected Mock<IConfiguration> Configuration;

        protected ServiceTestBase()
        {
            UserManager = GetMockUserManager();
            SignInManager = GetMockSignInManager();
            Configuration = GetMockConfiguration();
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

            var signInManager = new Mock<SignInManager<ApplicationUser>>(
                userManager.Object,
                contextAccessor.Object,
                claimsFactory.Object,
                options.Object,
                logger.Object,
                schemes.Object,
                confirmation.Object
            );

            signInManager
                .Setup(x => x.SignInAsync(It.IsAny<ApplicationUser>(), It.IsAny<bool>(), null))
                .Returns(Task.CompletedTask);

            return signInManager;
        }

        private static Mock<UserManager<ApplicationUser>> GetMockUserManager()
        {
            var store = new Mock<IUserStore<ApplicationUser>>();
            var options = new Mock<IOptions<IdentityOptions>>();
            var passwordHasher = new Mock<IPasswordHasher<ApplicationUser>>();
            var userValidators = new List<Mock<IUserValidator<ApplicationUser>>>();
            var passwordValidators = new List<Mock<IPasswordValidator<ApplicationUser>>>();
            var normalizer = new Mock<ILookupNormalizer>();
            var errorDescriber = new Mock<IdentityErrorDescriber>();
            var services = new Mock<IServiceProvider>();
            var logger = new Mock<ILogger<UserManager<ApplicationUser>>>();

            var userManager = new Mock<UserManager<ApplicationUser>>(
                store.Object,
                options.Object,
                passwordHasher.Object,
                userValidators.Select(v => v.Object).ToList(),
                passwordValidators.Select(v => v.Object).ToList(),
                normalizer.Object,
                errorDescriber.Object,
                services.Object,
                logger.Object
            );

            userManager
                .Setup(x => x.DeleteAsync(It.IsAny<ApplicationUser>()))
                .ReturnsAsync(IdentityResult.Success);
            userManager
                .Setup(x => x.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);
            userManager
                .Setup(x => x.UpdateAsync(It.IsAny<ApplicationUser>()))
                .ReturnsAsync(IdentityResult.Success);

            return userManager;
        }

        private static Mock<IConfiguration> GetMockConfiguration()
        {
            var mockConfiguration = new Mock<IConfiguration>();
            mockConfiguration
                .SetupGet(x => x["Jwt:Key"])
                .Returns("MegaSuperSecretKey1234567890123456");
            mockConfiguration.SetupGet(x => x["Jwt:Issuer"]).Returns("TestIssuer");
            mockConfiguration.SetupGet(x => x["Jwt:Audience"]).Returns("TestAudience");
            mockConfiguration.SetupGet(x => x["Jwt:ExpireMinutes"]).Returns("30");
            return mockConfiguration;
        }
    }
}

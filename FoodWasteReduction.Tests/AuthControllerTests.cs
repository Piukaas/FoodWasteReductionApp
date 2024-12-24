using System.Text.Json;
using FluentAssertions;
using FoodWasteReduction.Api.Controllers;
using FoodWasteReduction.Core.DTOs.Auth;
using FoodWasteReduction.Core.Entities;
using FoodWasteReduction.Infrastructure.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;

namespace FoodWasteReduction.Tests.Controllers
{
    public class AuthControllerTests
    {
        private readonly Mock<UserManager<ApplicationUser>> _mockUserManager;
        private readonly Mock<SignInManager<ApplicationUser>> _mockSignInManager;
        private readonly ApplicationDbContext _context;
        private readonly AuthController _controller;
        private readonly Mock<IConfiguration> _mockConfiguration;

        public AuthControllerTests()
        {
            var userStoreMock = new Mock<IUserStore<ApplicationUser>>();
            var options = new Mock<IOptions<IdentityOptions>>();
            var passwordHasher = new Mock<IPasswordHasher<ApplicationUser>>();
            var userValidators = new List<IUserValidator<ApplicationUser>>();
            var passwordValidators = new List<IPasswordValidator<ApplicationUser>>();
            var keyNormalizer = new Mock<ILookupNormalizer>();
            var errors = new Mock<IdentityErrorDescriber>();
            var services = new Mock<IServiceProvider>();
            var logger = new Mock<ILogger<UserManager<ApplicationUser>>>();

            _mockUserManager = new Mock<UserManager<ApplicationUser>>(
                userStoreMock.Object,
                options.Object,
                passwordHasher.Object,
                userValidators,
                passwordValidators,
                keyNormalizer.Object,
                errors.Object,
                services.Object,
                logger.Object
            );

            var contextAccessor = new Mock<IHttpContextAccessor>();
            var claimsFactory = new Mock<IUserClaimsPrincipalFactory<ApplicationUser>>();
            var loggerSignIn = new Mock<ILogger<SignInManager<ApplicationUser>>>();
            var schemes = new Mock<IAuthenticationSchemeProvider>();
            var confirmation = new Mock<IUserConfirmation<ApplicationUser>>();

            _mockSignInManager = new Mock<SignInManager<ApplicationUser>>(
                _mockUserManager.Object,
                contextAccessor.Object,
                claimsFactory.Object,
                options.Object,
                loggerSignIn.Object,
                schemes.Object,
                confirmation.Object
            );

            var dbContextOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .ConfigureWarnings(w => w.Ignore(InMemoryEventId.TransactionIgnoredWarning))
                .Options;

            _context = new ApplicationDbContext(dbContextOptions);

            _mockConfiguration = new Mock<IConfiguration>();
            _mockConfiguration
                .SetupGet(x => x["Jwt:Key"])
                .Returns("MegaSuperSecretKey1234567890123456");
            _mockConfiguration.SetupGet(x => x["Jwt:Issuer"]).Returns("FoodWasteReductionApp");
            _mockConfiguration.SetupGet(x => x["Jwt:Audience"]).Returns("http://localhost:5019");
            _mockConfiguration.SetupGet(x => x["Jwt:ExpireMinutes"]).Returns("30");

            _controller = new AuthController(
                _mockUserManager.Object,
                _mockSignInManager.Object,
                _context,
                _mockConfiguration.Object
            );
        }

        [Fact]
        public async Task RegisterStudent_WithDuplicateEmail_ReturnsBadRequest()
        {
            // Arrange
            var registerDto = new RegisterStudentDto
            {
                Email = "existing@example.com",
                Password = "ValidPass123!",
                Name = "Test User",
                StudentNumber = "S123456",
                DateOfBirth = new DateTime(2000, 1, 1),
                StudyCity = "Test City",
            };

            _mockUserManager
                .Setup(x => x.CreateAsync(It.IsAny<Student>(), It.IsAny<string>()))
                .ReturnsAsync(
                    IdentityResult.Failed(
                        new IdentityError { Description = "Email already exists" }
                    )
                );

            // Act
            var result = await _controller.RegisterStudent(registerDto);

            // Assert
            var badRequestResult = result.Should().BeOfType<BadRequestObjectResult>().Subject;
            var modelState = badRequestResult.Value as SerializableError;
            modelState.Should().ContainKey(string.Empty);
        }

        [Fact]
        public async Task RegisterStudent_WithShortPassword_ReturnsBadRequest()
        {
            // Arrange
            var registerDto = new RegisterStudentDto
            {
                Email = "test@example.com",
                Password = "short",
                Name = "Test User",
                StudentNumber = "S123456",
                DateOfBirth = new DateTime(2000, 1, 1),
                StudyCity = "Test City",
            };

            _mockUserManager
                .Setup(x => x.CreateAsync(It.IsAny<Student>(), It.IsAny<string>()))
                .ReturnsAsync(
                    IdentityResult.Failed(
                        new IdentityError { Description = "Password must be at least 8 characters" }
                    )
                );

            // Act
            var result = await _controller.RegisterStudent(registerDto);

            // Assert
            var badRequestResult = result.Should().BeOfType<BadRequestObjectResult>().Subject;
            var modelState = badRequestResult.Value as SerializableError;
            modelState.Should().ContainKey(string.Empty);
        }

        [Fact]
        public async Task RegisterStudent_WithUnderageStudent_ReturnsBadRequest()
        {
            // Arrange
            var registerDto = new RegisterStudentDto
            {
                Email = "test@example.com",
                Password = "ValidPass123!",
                Name = "Test User",
                StudentNumber = "S123456",
                DateOfBirth = DateTime.Now.AddYears(-15), // 15 years old
                StudyCity = "Test City",
            };

            _mockUserManager
                .Setup(x => x.CreateAsync(It.IsAny<Student>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);

            // Act
            var result = await _controller.RegisterStudent(registerDto);

            // Assert
            var badRequestResult = result.Should().BeOfType<BadRequestObjectResult>().Subject;
            var modelState = badRequestResult.Value as SerializableError;
            modelState!
                [string.Empty]
                .As<string[]>()
                .Should()
                .Contain(x => x.Contains("16 jaar") && x.Contains("toekomst"));
        }

        [Fact]
        public async Task RegisterStudent_WithFutureBirthDate_ReturnsBadRequest()
        {
            // Arrange
            var registerDto = new RegisterStudentDto
            {
                Email = "test@example.com",
                Password = "ValidPass123!",
                Name = "Test User",
                StudentNumber = "S123456",
                DateOfBirth = DateTime.Now.AddDays(1), // Future date
                StudyCity = "Test City",
            };

            _mockUserManager
                .Setup(x => x.CreateAsync(It.IsAny<Student>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);

            // Act
            var result = await _controller.RegisterStudent(registerDto);

            // Assert
            var badRequestResult = result.Should().BeOfType<BadRequestObjectResult>().Subject;
            var modelState = badRequestResult.Value as SerializableError;
            modelState.Should().ContainKey(string.Empty);
            modelState!
                [string.Empty]
                .As<string[]>()
                .Should()
                .Contain(x => x.Contains("16 jaar") && x.Contains("toekomst"));
        }

        [Fact]
        public async Task RegisterStudent_WithValidData_ReturnsOkResult()
        {
            // Arrange
            var registerDto = new RegisterStudentDto
            {
                Email = "valid@example.com",
                Password = "ValidPass123!",
                Name = "Valid User",
                StudentNumber = "S123456",
                DateOfBirth = DateTime.Now.AddYears(-20),
                StudyCity = "Test City",
                PhoneNumber = "1234567890",
            };

            _mockUserManager
                .Setup(x => x.CreateAsync(It.IsAny<Student>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);
            _mockUserManager
                .Setup(x => x.AddToRoleAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);

            // Act
            var result = await _controller.RegisterStudent(registerDto);

            // Assert
            result.Should().BeOfType<OkResult>();
        }

        [Fact]
        public async Task RegisterCanteenStaff_WithValidData_ReturnsOkResult()
        {
            // Arrange
            var registerDto = new RegisterCanteenStaffDto
            {
                Email = "staff@example.com",
                Password = "ValidPass123!",
                Name = "Staff User",
                PersonnelNumber = "P123456",
                Location = "Main Building",
            };

            _mockUserManager
                .Setup(x => x.CreateAsync(It.IsAny<CanteenStaff>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);
            _mockUserManager
                .Setup(x => x.AddToRoleAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);

            // Act
            var result = await _controller.RegisterCanteenStaff(registerDto);

            // Assert
            result.Should().BeOfType<OkResult>();
        }

        [Fact]
        public async Task Login_WithValidCredentials_ReturnsOkResultWithUserInfo()
        {
            // Arrange
            var loginDto = new LoginDto { Email = "test@example.com", Password = "ValidPass123!" };

            var user = new Student
            {
                Id = "123",
                Email = "test@example.com",
                Name = "Test User",
                StudentNumber = "S123456",
                DateOfBirth = DateTime.Now.AddYears(-20),
                StudyCity = "Test City",
                PhoneNumber = "1234567890",
            };

            _mockSignInManager
                .Setup(x =>
                    x.PasswordSignInAsync(
                        It.IsAny<string>(),
                        It.IsAny<string>(),
                        It.IsAny<bool>(),
                        It.IsAny<bool>()
                    )
                )
                .ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.Success);

            _mockUserManager.Setup(x => x.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync(user);
            _mockUserManager
                .Setup(x => x.GetRolesAsync(It.IsAny<ApplicationUser>()))
                .ReturnsAsync(new List<string> { "Student" });

            // Act
            var result = await _controller.Login(loginDto);

            // Assert
            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            okResult.Should().NotBeNull();
            var value = okResult.Value.Should().BeAssignableTo<object>().Subject;
            var json = JsonSerializer.Serialize(value);
            var userInfo = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(json);

            userInfo.Should().NotBeNull();
            userInfo.Should().ContainKey("Email");
            userInfo!["Email"].GetString().Should().Be("test@example.com");
            userInfo.Should().ContainKey("Name");
            userInfo!["Name"].GetString().Should().Be("Test User");
            userInfo.Should().ContainKey("DateOfBirth");

            if (userInfo["DateOfBirth"].ValueKind != JsonValueKind.Null)
            {
                var dateOfBirth = userInfo["DateOfBirth"].GetDateTime();
                dateOfBirth.Should().Be(user.DateOfBirth);
            }

            var roles = userInfo["Roles"].EnumerateArray().Select(r => r.GetString()).ToList();
            roles.Should().NotBeNull();
            roles.Should().Contain("Student");
        }
    }
}

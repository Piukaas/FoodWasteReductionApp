using System.Text.Json;
using FluentAssertions;
using FoodWasteReduction.Api.Controllers;
using FoodWasteReduction.Core.DTOs.Auth;
using FoodWasteReduction.Core.Entities;
using FoodWasteReduction.Core.Enums;
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
            var registerDTO = new RegisterStudentDTO
            {
                Email = "existing@example.com",
                Password = "ValidPass123!",
                Name = "Test User",
                StudentNumber = "S123456",
                DateOfBirth = new DateTime(2000, 1, 1),
                StudyCity = City.Breda,
            };

            _mockUserManager
                .Setup(x => x.CreateAsync(It.IsAny<Student>(), It.IsAny<string>()))
                .ReturnsAsync(
                    IdentityResult.Failed(
                        new IdentityError { Description = "Email already exists" }
                    )
                );

            // Act
            var result = await _controller.RegisterStudent(registerDTO);

            // Assert
            var badRequestResult = result.Should().BeOfType<BadRequestObjectResult>().Subject;
            var modelState = badRequestResult.Value as SerializableError;
            modelState.Should().ContainKey(string.Empty);
        }

        [Fact]
        public async Task RegisterStudent_WithShortPassword_ReturnsBadRequest()
        {
            // Arrange
            var registerDTO = new RegisterStudentDTO
            {
                Email = "test@example.com",
                Password = "short",
                Name = "Test User",
                StudentNumber = "S123456",
                DateOfBirth = new DateTime(2000, 1, 1),
                StudyCity = City.Breda,
            };

            _mockUserManager
                .Setup(x => x.CreateAsync(It.IsAny<Student>(), It.IsAny<string>()))
                .ReturnsAsync(
                    IdentityResult.Failed(
                        new IdentityError { Description = "Password must be at least 8 characters" }
                    )
                );

            // Act
            var result = await _controller.RegisterStudent(registerDTO);

            // Assert
            var badRequestResult = result.Should().BeOfType<BadRequestObjectResult>().Subject;
            var modelState = badRequestResult.Value as SerializableError;
            modelState.Should().ContainKey(string.Empty);
        }

        [Fact]
        public async Task RegisterStudent_WithUnderageStudent_ReturnsBadRequest()
        {
            // Arrange
            var registerDTO = new RegisterStudentDTO
            {
                Email = "test@example.com",
                Password = "ValidPass123!",
                Name = "Test User",
                StudentNumber = "S123456",
                DateOfBirth = DateTime.Now.AddYears(-15), // 15 years old
                StudyCity = City.Breda,
            };

            _mockUserManager
                .Setup(x => x.CreateAsync(It.IsAny<Student>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);

            // Act
            var result = await _controller.RegisterStudent(registerDTO);

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
            var registerDTO = new RegisterStudentDTO
            {
                Email = "test@example.com",
                Password = "ValidPass123!",
                Name = "Test User",
                StudentNumber = "S123456",
                DateOfBirth = DateTime.Now.AddDays(1), // Future date
                StudyCity = City.Breda,
            };

            _mockUserManager
                .Setup(x => x.CreateAsync(It.IsAny<Student>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);

            // Act
            var result = await _controller.RegisterStudent(registerDTO);

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
            var registerDTO = new RegisterStudentDTO
            {
                Email = "valid@example.com",
                Password = "ValidPass123!",
                Name = "Valid User",
                StudentNumber = "S123456",
                DateOfBirth = DateTime.Now.AddYears(-20),
                StudyCity = City.Breda,
                PhoneNumber = "1234567890",
            };

            _mockUserManager
                .Setup(x => x.CreateAsync(It.IsAny<Student>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);
            _mockUserManager
                .Setup(x => x.AddToRoleAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);

            // Act
            var result = await _controller.RegisterStudent(registerDTO);

            // Assert
            result.Should().BeOfType<OkResult>();
        }

        [Fact]
        public async Task RegisterCanteenStaff_WithValidData_ReturnsOkResult()
        {
            // Arrange
            var registerDTO = new RegisterCanteenStaffDTO
            {
                Email = "staff@example.com",
                Password = "ValidPass123!",
                Name = "Staff User",
                PersonnelNumber = "P123456",
                Location = Location.LA,
            };

            _mockUserManager
                .Setup(x => x.CreateAsync(It.IsAny<CanteenStaff>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);
            _mockUserManager
                .Setup(x => x.AddToRoleAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);

            // Act
            var result = await _controller.RegisterCanteenStaff(registerDTO);

            // Assert
            result.Should().BeOfType<OkResult>();
        }

        [Fact]
        public async Task Login_WithValidCredentials_ReturnsOkResultWithUserInfo()
        {
            // Arrange
            var loginDTO = new LoginDTO { Email = "test@example.com", Password = "ValidPass123!" };

            var user = new Student
            {
                Id = "123",
                Email = "test@example.com",
                Name = "Test User",
                StudentNumber = "S123456",
                DateOfBirth = DateTime.Now.AddYears(-20),
                StudyCity = City.Breda,
            };

            // Setup in-memory database
            _context.Students?.Add(user);
            await _context.SaveChangesAsync();

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
                .ReturnsAsync(["Student"]);

            // Act
            var result = await _controller.Login(loginDTO);

            // Assert
            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            okResult.Should().NotBeNull();

            var value = okResult.Value.Should().BeAssignableTo<object>().Subject;
            var json = JsonSerializer.Serialize(value);
            var response = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(json);

            response.Should().ContainKey("responseData");
            var responseData = response!["responseData"];

            responseData.TryGetProperty("Token", out var token);
            token.GetString().Should().NotBeNullOrEmpty();

            responseData.TryGetProperty("Roles", out var rolesElement);
            var roles = rolesElement.EnumerateArray().Select(r => r.GetString()).ToList();
            roles.Should().Contain("Student");

            // Optional: Check additionalData if present
            if (response.ContainsKey("AdditionalData"))
            {
                var additionalData = response["AdditionalData"];
                additionalData.TryGetProperty("StudyCity", out var city);
                city.GetInt32().Should().Be((int)City.Breda);
            }
        }
    }
}

using System.Text.Json;
using FluentAssertions;
using FoodWasteReduction.Application.DTOs.Auth;
using FoodWasteReduction.Application.Services;
using FoodWasteReduction.Core.Constants;
using FoodWasteReduction.Core.Entities;
using FoodWasteReduction.Core.Enums;
using FoodWasteReduction.Core.Interfaces.Repositories;
using FoodWasteReduction.Tests.Services;
using Microsoft.AspNetCore.Identity;
using Moq;

namespace FoodWasteReduction.Tests.Services.Application
{
    public class AuthServiceTests : ServiceTestBase
    {
        private readonly Mock<IStudentRepository> _mockStudentRepository;
        private readonly Mock<ICanteenStaffRepository> _mockCanteenStaffRepository;
        private readonly AuthService _service;

        public AuthServiceTests()
        {
            _mockStudentRepository = new Mock<IStudentRepository>();
            _mockCanteenStaffRepository = new Mock<ICanteenStaffRepository>();

            _service = new AuthService(
                UserManager.Object,
                SignInManager.Object,
                _mockStudentRepository.Object,
                _mockCanteenStaffRepository.Object,
                Configuration.Object
            );
        }

        [Fact]
        public async Task RegisterStudent_WithValidData_ReturnsSuccess()
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

            UserManager
                .Setup(x => x.CreateAsync(It.IsAny<ApplicationUser>(), registerDTO.Password))
                .ReturnsAsync(IdentityResult.Success);

            UserManager
                .Setup(x => x.AddToRoleAsync(It.IsAny<ApplicationUser>(), Roles.Student))
                .ReturnsAsync(IdentityResult.Success);

            _mockStudentRepository
                .Setup(x => x.CreateAsync(It.IsAny<Student>()))
                .ReturnsAsync(new Student());

            // Act
            var (success, error) = await _service.RegisterStudentAsync(registerDTO);

            // Assert
            success.Should().BeTrue();
            error.Should().BeNull();
            _mockStudentRepository.Verify(x => x.CreateAsync(It.IsAny<Student>()), Times.Once);
        }

        [Fact]
        public async Task RegisterCanteenStaff_WithValidData_ReturnsSuccess()
        {
            // Arrange
            var registerDTO = new RegisterCanteenStaffDTO
            {
                Email = "staff@example.com",
                Password = "ValidPass123!",
                Name = "Staff User",
                PersonnelNumber = "P123456",
                CanteenId = 1,
            };

            UserManager
                .Setup(x => x.CreateAsync(It.IsAny<ApplicationUser>(), registerDTO.Password))
                .ReturnsAsync(IdentityResult.Success);

            UserManager
                .Setup(x => x.AddToRoleAsync(It.IsAny<ApplicationUser>(), Roles.CanteenStaff))
                .ReturnsAsync(IdentityResult.Success);

            _mockCanteenStaffRepository
                .Setup(x => x.CreateAsync(It.IsAny<CanteenStaff>()))
                .ReturnsAsync(new CanteenStaff());

            // Act
            var (success, error) = await _service.RegisterCanteenStaffAsync(registerDTO);

            // Assert
            success.Should().BeTrue();
            error.Should().BeNull();
            _mockCanteenStaffRepository.Verify(
                x => x.CreateAsync(It.IsAny<CanteenStaff>()),
                Times.Once
            );
        }

        [Fact]
        public async Task Login_WithValidCredentials_ReturnsSuccessAndToken()
        {
            // Arrange
            var loginDTO = new LoginDTO { Email = "test@example.com", Password = "ValidPass123!" };
            var user = new ApplicationUser
            {
                Id = "testId",
                Email = loginDTO.Email,
                UserName = loginDTO.Email,
                Name = "Test User",
            };

            SignInManager
                .Setup(x => x.PasswordSignInAsync(loginDTO.Email, loginDTO.Password, false, false))
                .ReturnsAsync(SignInResult.Success);

            UserManager.Setup(x => x.FindByEmailAsync(loginDTO.Email)).ReturnsAsync(user);

            UserManager
                .Setup(x => x.GetRolesAsync(user))
                .ReturnsAsync(new List<string> { Roles.Student });

            // Act
            var (success, response, error) = await _service.LoginAsync(loginDTO);

            // Assert
            success.Should().BeTrue();
            error.Should().BeNull();
            response.Should().NotBeNull();

            var responseDict = JsonSerializer.Serialize(response);
            var parsed = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(responseDict);

            parsed.Should().ContainKey("responseData");
            var responseData = parsed!["responseData"];
            responseData.TryGetProperty("Token", out var token);
            token.GetString().Should().NotBeNullOrEmpty();
        }

        [Fact]
        public async Task Login_WithInvalidCredentials_ReturnsError()
        {
            // Arrange
            var loginDTO = new LoginDTO { Email = "test@example.com", Password = "WrongPass123!" };

            SignInManager
                .Setup(x => x.PasswordSignInAsync(loginDTO.Email, loginDTO.Password, false, false))
                .ReturnsAsync(SignInResult.Failed);

            // Act
            var (success, response, error) = await _service.LoginAsync(loginDTO);

            // Assert
            success.Should().BeFalse();
            response.Should().BeNull();
            error.Should().Be("Invalid login credentials");
        }

        [Fact]
        public async Task RegisterStudent_WithDuplicateEmail_ReturnsError()
        {
            // Arrange
            var registerDTO = new RegisterStudentDTO
            {
                Email = "existing@example.com",
                Password = "ValidPass123!",
            };

            UserManager
                .Setup(x => x.CreateAsync(It.IsAny<ApplicationUser>(), registerDTO.Password))
                .ReturnsAsync(
                    IdentityResult.Failed(
                        new IdentityError { Description = "Email already exists" }
                    )
                );

            // Act
            var (success, error) = await _service.RegisterStudentAsync(registerDTO);

            // Assert
            success.Should().BeFalse();
            error.Should().Be("Email already exists");
            _mockStudentRepository.Verify(x => x.CreateAsync(It.IsAny<Student>()), Times.Never);
        }
    }
}

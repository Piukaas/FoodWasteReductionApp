using System.Text.Json;
using FluentAssertions;
using FoodWasteReduction.Api.Controllers;
using FoodWasteReduction.Application.DTOs.Auth;
using FoodWasteReduction.Core.Entities;
using FoodWasteReduction.Core.Enums;
using FoodWasteReduction.Core.Interfaces.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq;

namespace FoodWasteReduction.Tests.Controllers.Api
{
    public class AuthControllerTests : ApiControllerTestBase
    {
        private readonly Mock<IStudentRepository> _mockStudentRepository;
        private readonly Mock<ICanteenStaffRepository> _mockCanteenStaffRepository;
        private readonly Mock<IConfiguration> _mockConfiguration;
        private readonly AuthController _controller;

        public AuthControllerTests()
            : base()
        {
            _mockStudentRepository = new Mock<IStudentRepository>();
            _mockCanteenStaffRepository = new Mock<ICanteenStaffRepository>();

            _mockConfiguration = new Mock<IConfiguration>();
            _mockConfiguration
                .SetupGet(x => x["Jwt:Key"])
                .Returns("MegaSuperSecretKey1234567890123456");
            _mockConfiguration.SetupGet(x => x["Jwt:Issuer"]).Returns("FoodWasteReductionApp");
            _mockConfiguration.SetupGet(x => x["Jwt:Audience"]).Returns("http://localhost:5019");
            _mockConfiguration.SetupGet(x => x["Jwt:ExpireMinutes"]).Returns("30");

            _controller = new AuthController(
                UserManager.Object,
                SignInManager.Object,
                _mockStudentRepository.Object,
                _mockCanteenStaffRepository.Object,
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

            UserManager
                .Setup(x => x.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
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
            _mockStudentRepository.Verify(x => x.CreateAsync(It.IsAny<Student>()), Times.Never);
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

            UserManager
                .Setup(x => x.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
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
            _mockStudentRepository.Verify(x => x.CreateAsync(It.IsAny<Student>()), Times.Never);
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
            UserManager.Verify(
                x => x.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()),
                Times.Never
            );
            _mockStudentRepository.Verify(x => x.CreateAsync(It.IsAny<Student>()), Times.Never);
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
            UserManager.Verify(
                x => x.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()),
                Times.Never
            );
            _mockStudentRepository.Verify(x => x.CreateAsync(It.IsAny<Student>()), Times.Never);
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

            UserManager
                .Setup(x => x.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);
            UserManager
                .Setup(x => x.AddToRoleAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);
            _mockStudentRepository
                .Setup(x => x.CreateAsync(It.IsAny<Student>()))
                .ReturnsAsync(new Student());

            // Act
            var result = await _controller.RegisterStudent(registerDTO);

            // Assert
            result.Should().BeOfType<OkResult>();
            _mockStudentRepository.Verify(x => x.CreateAsync(It.IsAny<Student>()), Times.Once);
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
                CanteenId = 1,
            };

            UserManager
                .Setup(x => x.CreateAsync(It.IsAny<ApplicationUser>(), registerDTO.Password))
                .ReturnsAsync(IdentityResult.Success);

            UserManager
                .Setup(x => x.AddToRoleAsync(It.IsAny<ApplicationUser>(), "CanteenStaff"))
                .ReturnsAsync(IdentityResult.Success);

            _mockCanteenStaffRepository
                .Setup(x =>
                    x.CreateAsync(
                        It.Is<CanteenStaff>(cs =>
                            cs.PersonnelNumber == registerDTO.PersonnelNumber
                            && cs.CanteenId == registerDTO.CanteenId
                        )
                    )
                )
                .ReturnsAsync(new CanteenStaff());

            // Act
            var result = await _controller.RegisterCanteenStaff(registerDTO);

            // Assert
            result.Should().BeOfType<OkResult>();
            _mockCanteenStaffRepository.Verify(
                x => x.CreateAsync(It.IsAny<CanteenStaff>()),
                Times.Once
            );
        }

        [Fact]
        public async Task Login_WithValidCredentials_ReturnsOkResultWithUserInfo()
        {
            // Arrange
            var loginDTO = new LoginDTO { Email = "test@example.com", Password = "ValidPass123!" };

            var identityUser = new ApplicationUser
            {
                Id = "testId",
                Email = loginDTO.Email,
                UserName = loginDTO.Email,
                Name = "Test User",
            };

            var birthday = DateTime.Now.AddYears(-20);
            var student = new Student
            {
                Id = "testId",
                DateOfBirth = birthday,
                StudyCity = City.Breda,
                StudentNumber = "S123456",
            };

            SignInManager
                .Setup(x => x.PasswordSignInAsync(loginDTO.Email, loginDTO.Password, false, false))
                .ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.Success);

            UserManager.Setup(x => x.FindByEmailAsync(loginDTO.Email)).ReturnsAsync(identityUser);

            UserManager.Setup(x => x.GetRolesAsync(identityUser)).ReturnsAsync(["Student"]);

            _mockStudentRepository.Setup(x => x.GetByIdAsync("testId")).ReturnsAsync(student);

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

            responseData.TryGetProperty("Token", out JsonElement token);
            token.GetString().Should().NotBeNullOrEmpty();

            responseData.TryGetProperty("Roles", out JsonElement rolesElement);
            var roles = rolesElement.EnumerateArray().Select(r => r.GetString()).ToList();
            roles.Should().Contain("Student");

            if (response.ContainsKey("AdditionalData"))
            {
                var additionalData = response["AdditionalData"];
                additionalData.TryGetProperty("StudyCity", out JsonElement city);
                city.GetInt32().Should().Be((int)City.Breda);
                additionalData.TryGetProperty("DateOfBirth", out JsonElement dateOfBirth);
                dateOfBirth.GetDateTime().Should().Be(birthday);
            }
        }
    }
}

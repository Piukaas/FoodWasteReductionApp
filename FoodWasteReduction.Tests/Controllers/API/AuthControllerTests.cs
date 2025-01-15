using FluentAssertions;
using FoodWasteReduction.Api.Controllers;
using FoodWasteReduction.Application.DTOs.Auth;
using FoodWasteReduction.Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace FoodWasteReduction.Tests.Controllers.Api
{
    public class AuthControllerTests : ApiControllerTestBase
    {
        private readonly Mock<IAuthService> _mockAuthService;
        private readonly AuthController _controller;

        public AuthControllerTests()
            : base()
        {
            _mockAuthService = new Mock<IAuthService>();
            _controller = new AuthController(_mockAuthService.Object);
            SetupController(_controller);
        }

        [Fact]
        public async Task RegisterStudent_WithValidData_ReturnsOkResult()
        {
            // Arrange
            var registerDto = new RegisterStudentDTO
            {
                Email = "test@example.com",
                Password = "Password123!",
                Name = "Test User",
            };

            _mockAuthService
                .Setup(x => x.RegisterStudentAsync(registerDto))
                .ReturnsAsync((true, null));

            // Act
            var result = await _controller.RegisterStudent(registerDto);

            // Assert
            result.Should().BeOfType<OkResult>();
            _mockAuthService.Verify(x => x.RegisterStudentAsync(registerDto), Times.Once);
        }

        [Fact]
        public async Task RegisterStudent_WithInvalidModel_ReturnsBadRequest()
        {
            // Arrange
            var registerDto = new RegisterStudentDTO();
            _controller.ModelState.AddModelError("Email", "Required");

            // Act
            var result = await _controller.RegisterStudent(registerDto);

            // Assert
            var badRequestResult = result.Should().BeOfType<BadRequestObjectResult>().Subject;
            _mockAuthService.Verify(
                x => x.RegisterStudentAsync(It.IsAny<RegisterStudentDTO>()),
                Times.Never
            );
        }

        [Fact]
        public async Task RegisterCanteenStaff_WithValidData_ReturnsOkResult()
        {
            // Arrange
            var registerDto = new RegisterCanteenStaffDTO
            {
                Email = "staff@example.com",
                Password = "Password123!",
                Name = "Staff User",
                PersonnelNumber = "P123456",
                CanteenId = 1,
            };

            _mockAuthService
                .Setup(x => x.RegisterCanteenStaffAsync(registerDto))
                .ReturnsAsync((true, null));

            // Act
            var result = await _controller.RegisterCanteenStaff(registerDto);

            // Assert
            result.Should().BeOfType<OkResult>();
            _mockAuthService.Verify(x => x.RegisterCanteenStaffAsync(registerDto), Times.Once);
        }

        [Fact]
        public async Task RegisterCanteenStaff_WithInvalidModel_ReturnsBadRequest()
        {
            // Arrange
            var registerDto = new RegisterCanteenStaffDTO();
            _controller.ModelState.AddModelError("Email", "Required");

            // Act
            var result = await _controller.RegisterCanteenStaff(registerDto);

            // Assert
            var badRequestResult = result.Should().BeOfType<BadRequestObjectResult>().Subject;
            _mockAuthService.Verify(
                x => x.RegisterCanteenStaffAsync(It.IsAny<RegisterCanteenStaffDTO>()),
                Times.Never
            );
        }

        [Fact]
        public async Task Login_WithValidCredentials_ReturnsOkResult()
        {
            // Arrange
            var loginDto = new LoginDTO { Email = "test@example.com", Password = "Password123!" };
            var expectedResponse = new { Token = "jwt_token" };

            _mockAuthService
                .Setup(x => x.LoginAsync(loginDto))
                .ReturnsAsync((true, expectedResponse, null));

            // Act
            var result = await _controller.Login(loginDto);

            // Assert
            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            okResult.Value.Should().Be(expectedResponse);
            _mockAuthService.Verify(x => x.LoginAsync(loginDto), Times.Once);
        }

        [Fact]
        public async Task Login_WithInvalidCredentials_ReturnsBadRequest()
        {
            // Arrange
            var loginDto = new LoginDTO { Email = "test@example.com", Password = "WrongPassword" };

            _mockAuthService
                .Setup(x => x.LoginAsync(loginDto))
                .ReturnsAsync((false, null, "Invalid credentials"));

            // Act
            var result = await _controller.Login(loginDto);

            // Assert
            var badRequestResult = result.Should().BeOfType<BadRequestObjectResult>().Subject;
            badRequestResult.Value.Should().Be("Invalid credentials");
            _mockAuthService.Verify(x => x.LoginAsync(loginDto), Times.Once);
        }
    }
}

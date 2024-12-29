using FluentAssertions;
using FoodWasteReduction.Web.Controllers;
using FoodWasteReduction.Web.Models.Auth;
using FoodWasteReduction.Web.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace FoodWasteReduction.Tests.Controllers.Web
{
    public class AuthControllerTests : ControllerTestBase
    {
        private readonly Mock<IAuthService> _authService;
        private readonly Mock<IAuthGuardService> _authGuardService;
        private readonly AuthController _controller;

        public AuthControllerTests()
        {
            _authService = new Mock<IAuthService>();
            _authGuardService = new Mock<IAuthGuardService>();
            _controller = new AuthController(_authService.Object, _authGuardService.Object)
            {
                ControllerContext = new ControllerContext { HttpContext = HttpContext },
            };
        }

        [Fact]
        public void Login_Get_WhenAuthenticated_RedirectsToHome()
        {
            // Arrange
            _authGuardService.Setup(x => x.IsAuthenticated).Returns(true);

            // Act
            var result = _controller.Login();

            // Assert
            var redirectResult = result.Should().BeOfType<RedirectToActionResult>().Subject;
            redirectResult.ActionName.Should().Be("Index");
            redirectResult.ControllerName.Should().Be("Home");
        }

        [Fact]
        public void Login_Get_WhenNotAuthenticated_ReturnsView()
        {
            // Arrange
            _authGuardService.Setup(x => x.IsAuthenticated).Returns(false);

            // Act
            var result = _controller.Login();

            // Assert
            result.Should().BeOfType<ViewResult>();
        }

        [Fact]
        public async Task Login_Post_WithValidModel_RedirectsToHome()
        {
            // Arrange
            var model = new LoginViewModel { Email = "test@test.com", Password = "password" };
            var userData = new { Id = "1", Role = "Student" };
            _authService.Setup(x => x.Login(model)).ReturnsAsync((true, "token", userData));

            // Act
            var result = await _controller.Login(model);

            // Assert
            var redirectResult = result.Should().BeOfType<RedirectToActionResult>().Subject;
            redirectResult.ActionName.Should().Be("Index");
            redirectResult.ControllerName.Should().Be("Home");
        }

        [Fact]
        public async Task Login_Post_WithInvalidModel_ReturnsView()
        {
            // Arrange
            var model = new LoginViewModel();
            _controller.ModelState.AddModelError("", "Test error");

            // Act
            var result = await _controller.Login(model);

            // Assert
            var viewResult = result.Should().BeOfType<ViewResult>().Subject;
            viewResult.Model.Should().Be(model);
        }

        [Fact]
        public void Logout_ClearsSessionAndRedirectsToHome()
        {
            // Act
            var result = _controller.Logout();

            // Assert
            _authGuardService.Verify(x => x.ClearToken(), Times.Once);
            Session.Verify(x => x.Remove("JWTToken"), Times.Once);
            Session.Verify(x => x.Remove("UserData"), Times.Once);
            var redirectResult = result.Should().BeOfType<RedirectToActionResult>().Subject;
            redirectResult.ActionName.Should().Be("Index");
            redirectResult.ControllerName.Should().Be("Home");
        }

        [Fact]
        public async Task RegisterStudent_Post_WithValidModel_RedirectsToLogin()
        {
            // Arrange
            var model = new RegisterStudentViewModel
            {
                Email = "test@test.com",
                Password = "password",
                ConfirmPassword = "password",
            };
            _authService.Setup(x => x.RegisterStudent(model)).ReturnsAsync(true);

            // Act
            var result = await _controller.RegisterStudent(model);

            // Assert
            var redirectResult = result.Should().BeOfType<RedirectToActionResult>().Subject;
            redirectResult.ActionName.Should().Be("Login");
        }

        [Fact]
        public async Task RegisterCanteenStaff_Post_WithValidModel_RedirectsToLogin()
        {
            // Arrange
            var model = new RegisterCanteenStaffViewModel
            {
                Email = "staff@test.com",
                Password = "password",
                ConfirmPassword = "password",
            };
            _authService.Setup(x => x.RegisterCanteenStaff(model)).ReturnsAsync(true);

            // Act
            var result = await _controller.RegisterCanteenStaff(model);

            // Assert
            var redirectResult = result.Should().BeOfType<RedirectToActionResult>().Subject;
            redirectResult.ActionName.Should().Be("Login");
        }

        [Fact]
        public void Forbidden_ReturnsView()
        {
            // Act
            var result = _controller.Forbidden();

            // Assert
            result.Should().BeOfType<ViewResult>();
        }
    }
}

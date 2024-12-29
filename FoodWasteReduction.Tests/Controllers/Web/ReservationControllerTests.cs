using System.Text.Json;
using FluentAssertions;
using FoodWasteReduction.Web.Controllers;
using FoodWasteReduction.Web.Models;
using FoodWasteReduction.Web.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace FoodWasteReduction.Tests.Controllers.Web
{
    public class ReservationControllerTests : ControllerTestBase
    {
        private readonly Mock<IReservationService> _reservationService;
        private readonly ReservationController _controller;

        public ReservationControllerTests()
        {
            _reservationService = new Mock<IReservationService>();
            _controller = new ReservationController(_reservationService.Object)
            {
                ControllerContext = new ControllerContext { HttpContext = HttpContext },
            };
        }

        [Fact]
        public async Task ReservePackage_WithValidRequest_ReturnsOk()
        {
            // Arrange
            var request = new ReservePackageRequest { PackageId = 1 };
            SetupUserSession("user1", "Student");
            _reservationService.Setup(s => s.ReservePackage(1, "user1")).ReturnsAsync((true, null));

            // Act
            var result = await _controller.ReservePackage(request);

            // Assert
            result.Should().BeOfType<OkResult>();
        }

        [Fact]
        public async Task ReservePackage_WithNullRequest_ReturnsBadRequest()
        {
            // Act
            var result = await _controller.ReservePackage(null!);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task ReservePackage_WithoutUserData_ReturnsUnauthorized()
        {
            // Arrange
            var request = new ReservePackageRequest { PackageId = 1 };

            // Act
            var result = await _controller.ReservePackage(request);

            // Assert
            result.Should().BeOfType<UnauthorizedResult>();
        }

        [Fact]
        public async Task ReservePackage_WithMissingUserId_ReturnsUnauthorized()
        {
            // Arrange
            var request = new ReservePackageRequest { PackageId = 1 };
            SetupUserSession(null!, "Student");

            // Act
            var result = await _controller.ReservePackage(request);

            // Assert
            var unauthorizedResult = result.Should().BeOfType<UnauthorizedObjectResult>().Subject;
            unauthorizedResult.Value.Should().Be("User ID not found in session");
        }

        [Fact]
        public async Task ReservePackage_WhenServiceFails_ReturnsBadRequest()
        {
            // Arrange
            var request = new ReservePackageRequest { PackageId = 1 };
            SetupUserSession("user1", "Student");
            _reservationService
                .Setup(s => s.ReservePackage(1, "user1"))
                .ReturnsAsync((false, "Package already reserved"));

            // Act
            var result = await _controller.ReservePackage(request);

            // Assert
            var badRequestResult = result.Should().BeOfType<BadRequestObjectResult>().Subject;
            var json = JsonSerializer.Serialize(badRequestResult.Value);
            json.Should().Contain("Package already reserved");
        }
    }
}

using System.Net;
using FluentAssertions;
using FoodWasteReduction.Web.Services;
using FoodWasteReduction.Web.Services.Interfaces;
using Moq;

namespace FoodWasteReduction.Tests.Services
{
    public class ReservationServiceTests : ServiceTestBase
    {
        private readonly ReservationService _service;

        public ReservationServiceTests()
        {
            _service = new ReservationService(HttpClientFactory.Object, AuthGuardService.Object);
        }

        [Fact]
        public async Task ReservePackage_WithValidData_ReturnsSuccess()
        {
            // Arrange
            var packageId = 1;
            var userId = "user123";
            SetupHttpResponse(HttpStatusCode.OK);

            // Act
            var result = await _service.ReservePackage(packageId, userId);

            // Assert
            result.success.Should().BeTrue();
            result.errorMessage.Should().BeNull();
        }

        [Fact]
        public async Task ReservePackage_WithServerError_ReturnsError()
        {
            // Arrange
            var packageId = 1;
            var userId = "user123";
            var errorResponse = new { Message = "Package already reserved" };
            SetupHttpResponse(HttpStatusCode.BadRequest, errorResponse);

            // Act
            var result = await _service.ReservePackage(packageId, userId);

            // Assert
            result.success.Should().BeFalse();
            result.errorMessage.Should().Be("Package already reserved");
        }

        [Fact]
        public async Task ReservePackage_WithException_ReturnsError()
        {
            // Arrange
            var packageId = 1;
            var userId = "user123";

            // Act
            var result = await _service.ReservePackage(packageId, userId);

            // Assert
            result.success.Should().BeFalse();
            result
                .errorMessage.Should()
                .Be("Er is iets misgegaan bij het reserveren van het pakket");
        }

        [Fact]
        public async Task ReservePackage_WithoutToken_ReturnsError()
        {
            // Arrange
            SetupUnauthorizedAccess();
            var packageId = 1;
            var userId = "user123";

            // Act
            var result = await _service.ReservePackage(packageId, userId);

            // Assert
            result.success.Should().BeFalse();
            result
                .errorMessage.Should()
                .Be("Er is iets misgegaan bij het reserveren van het pakket");
        }
    }
}

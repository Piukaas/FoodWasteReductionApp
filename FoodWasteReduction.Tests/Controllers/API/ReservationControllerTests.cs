using FluentAssertions;
using FoodWasteReduction.Api.Controllers;
using FoodWasteReduction.Application.DTOs;
using FoodWasteReduction.Application.Services.Interfaces;
using FoodWasteReduction.Core.Entities;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace FoodWasteReduction.Tests.Controllers.Api
{
    public class ReservationControllerTests : ApiControllerTestBase
    {
        private readonly Mock<IReservationService> _mockReservationService;
        private readonly ReservationController _controller;

        public ReservationControllerTests()
        {
            _mockReservationService = new Mock<IReservationService>();
            _controller = new ReservationController(_mockReservationService.Object);
            SetupController(_controller);
        }

        [Fact]
        public async Task ReservePackage_WithoutStudentRole_ReturnsForbidden()
        {
            // Arrange
            SetupUserRole("CanteenStaff", _controller);
            var dto = new ReservePackageDTO { PackageId = 1, UserId = "user1" };

            // Act
            var result = await _controller.ReservePackage(dto);

            // Assert
            result.Should().BeOfType<ForbidResult>();
        }

        [Fact]
        public async Task ReservePackage_WithSuccess_ReturnsOkWithPackage()
        {
            // Arrange
            SetupUserRole("Student", _controller);
            var dto = new ReservePackageDTO { PackageId = 1, UserId = "user1" };
            var packageDto = new PackageDTO(new Package { Id = 1, ReservedById = "user1" });

            _mockReservationService
                .Setup(s => s.ReservePackageAsync(dto))
                .ReturnsAsync((true, packageDto, null));

            // Act
            var result = await _controller.ReservePackage(dto);

            // Assert
            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            okResult.Value.Should().Be(packageDto);
        }

        [Fact]
        public async Task ReservePackage_WithNotFoundError_ReturnsNotFound()
        {
            // Arrange
            SetupUserRole("Student", _controller);
            var dto = new ReservePackageDTO { PackageId = 1, UserId = "user1" };
            var error = new ErrorResponse { Code = "NOT_FOUND", Message = "Package not found" };

            _mockReservationService
                .Setup(s => s.ReservePackageAsync(dto))
                .ReturnsAsync((false, null, error));

            // Act
            var result = await _controller.ReservePackage(dto);

            // Assert
            var notFound = result.Should().BeOfType<NotFoundObjectResult>().Subject;
            notFound.Value.Should().Be("Package not found");
        }

        [Fact]
        public async Task ReservePackage_WithOtherError_ReturnsBadRequest()
        {
            // Arrange
            SetupUserRole("Student", _controller);
            var dto = new ReservePackageDTO { PackageId = 1, UserId = "user1" };
            var error = new ErrorResponse
            {
                Code = "ALREADY_RESERVED",
                Message = "Package already reserved",
            };

            _mockReservationService
                .Setup(s => s.ReservePackageAsync(dto))
                .ReturnsAsync((false, null, error));

            // Act
            var result = await _controller.ReservePackage(dto);

            // Assert
            var badRequest = result.Should().BeOfType<BadRequestObjectResult>().Subject;
            badRequest.Value.Should().Be(error);
        }
    }
}

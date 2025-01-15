using FluentAssertions;
using FoodWasteReduction.Api.Controllers;
using FoodWasteReduction.Application.DTOs;
using FoodWasteReduction.Application.Services.Interfaces;
using FoodWasteReduction.Core.Entities;
using FoodWasteReduction.Core.Enums;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace FoodWasteReduction.Tests.Controllers.Api
{
    public class PackagesControllerTests : ApiControllerTestBase
    {
        private readonly Mock<IPackageService> _mockPackageService;
        private readonly PackagesController _controller;

        public PackagesControllerTests()
        {
            _mockPackageService = new Mock<IPackageService>();
            _controller = new PackagesController(_mockPackageService.Object);
            SetupController(_controller);
        }

        [Fact]
        public async Task Create_WithoutAuthorization_ReturnsForbidden()
        {
            // Arrange
            var dto = new CreatePackageDTO();

            // Act
            var result = await _controller.Create(dto);

            // Assert
            result.Result.Should().BeOfType<ForbidResult>();
        }

        [Fact]
        public async Task Create_WithAuthorization_ReturnsCreatedPackage()
        {
            // Arrange
            SetupUserRole("CanteenStaff", _controller);
            var dto = new CreatePackageDTO
            {
                Name = "Test Package",
                Type = MealType.Warm,
                PickupTime = DateTime.Now.AddDays(1),
                Price = 5.99m,
            };
            var package = new PackageDTO(new Package { Id = 1, Name = dto.Name });

            _mockPackageService.Setup(x => x.CreateAsync(dto)).ReturnsAsync((true, package, null));

            // Act
            var result = await _controller.Create(dto);

            // Assert
            var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
            okResult.Value.Should().Be(package);
        }

        [Fact]
        public async Task Create_WithServiceError_ReturnsBadRequest()
        {
            // Arrange
            SetupUserRole("CanteenStaff", _controller);
            var dto = new CreatePackageDTO();
            _mockPackageService
                .Setup(x => x.CreateAsync(dto))
                .ReturnsAsync((false, null, "Error message"));

            // Act
            var result = await _controller.Create(dto);

            // Assert
            var badRequest = result.Result.Should().BeOfType<BadRequestObjectResult>().Subject;
            badRequest.Value.Should().Be("Error message");
        }

        [Fact]
        public async Task Delete_WithoutAuthorization_ReturnsForbidden()
        {
            // Act
            var result = await _controller.Delete(1);

            // Assert
            result.Should().BeOfType<ForbidResult>();
        }

        [Fact]
        public async Task Delete_WithAuthorization_ReturnsNoContent()
        {
            // Arrange
            SetupUserRole("CanteenStaff", _controller);
            _mockPackageService.Setup(x => x.DeleteAsync(1)).ReturnsAsync((true, null));

            // Act
            var result = await _controller.Delete(1);

            // Assert
            result.Should().BeOfType<NoContentResult>();
        }

        [Fact]
        public async Task Delete_WithServiceError_ReturnsBadRequest()
        {
            // Arrange
            SetupUserRole("CanteenStaff", _controller);
            _mockPackageService.Setup(x => x.DeleteAsync(1)).ReturnsAsync((false, "Error message"));

            // Act
            var result = await _controller.Delete(1);

            // Assert
            var badRequest = result.Should().BeOfType<BadRequestObjectResult>().Subject;
            badRequest.Value.Should().Be("Error message");
        }

        [Fact]
        public async Task Update_WithoutAuthorization_ReturnsForbidden()
        {
            // Arrange
            var dto = new CreatePackageDTO();

            // Act
            var result = await _controller.Update(1, dto);

            // Assert
            result.Result.Should().BeOfType<ForbidResult>();
        }

        [Fact]
        public async Task Update_WithAuthorization_ReturnsUpdatedPackage()
        {
            // Arrange
            SetupUserRole("CanteenStaff", _controller);
            var dto = new CreatePackageDTO { Name = "Updated Package" };
            var package = new PackageDTO(new Package { Id = 1, Name = dto.Name });

            _mockPackageService
                .Setup(x => x.UpdateAsync(1, dto))
                .ReturnsAsync((true, package, null));

            // Act
            var result = await _controller.Update(1, dto);

            // Assert
            var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
            okResult.Value.Should().Be(package);
        }

        [Fact]
        public async Task Update_WithServiceError_ReturnsBadRequest()
        {
            // Arrange
            SetupUserRole("CanteenStaff", _controller);
            var dto = new CreatePackageDTO();
            _mockPackageService
                .Setup(x => x.UpdateAsync(1, dto))
                .ReturnsAsync((false, null, "Error message"));

            // Act
            var result = await _controller.Update(1, dto);

            // Assert
            var badRequest = result.Result.Should().BeOfType<BadRequestObjectResult>().Subject;
            badRequest.Value.Should().Be("Error message");
        }
    }
}

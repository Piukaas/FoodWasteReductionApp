using FluentAssertions;
using FoodWasteReduction.Api.Controllers;
using FoodWasteReduction.Application.DTOs;
using FoodWasteReduction.Core.Entities;
using FoodWasteReduction.Core.Enums;
using FoodWasteReduction.Core.Interfaces.Repositories;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace FoodWasteReduction.Tests.Controllers.Api
{
    public class PackagesControllerTests : ApiControllerTestBase
    {
        private readonly Mock<IPackageRepository> _packageRepository;
        private readonly Mock<ICanteenRepository> _canteenRepository;
        private readonly Mock<IProductRepository> _productRepository;
        private readonly PackagesController _controller;

        public PackagesControllerTests()
        {
            _packageRepository = new Mock<IPackageRepository>();
            _canteenRepository = new Mock<ICanteenRepository>();
            _productRepository = new Mock<IProductRepository>();

            _controller = new PackagesController(
                _packageRepository.Object,
                _canteenRepository.Object,
                _productRepository.Object
            );
            SetupController(_controller);
        }

        [Fact]
        public async Task Create_WithoutAuthorization_ReturnsForbidden()
        {
            var dto = new CreatePackageDTO();
            var result = await _controller.Create(dto);
            result.Result.Should().BeOfType<ForbidResult>();
        }

        [Fact]
        public async Task Update_WithoutAuthorization_ReturnsForbidden()
        {
            var dto = new CreatePackageDTO();
            var result = await _controller.Update(1, dto);
            result.Result.Should().BeOfType<ForbidResult>();
        }

        [Fact]
        public async Task Delete_WithoutAuthorization_ReturnsForbidden()
        {
            var result = await _controller.Delete(1);
            result.Should().BeOfType<ForbidResult>();
        }

        [Fact]
        public async Task Create_WithInvalidCanteen_ReturnsBadRequest()
        {
            // Arrange
            SetupUserRole("CanteenStaff", _controller);
            var dto = new CreatePackageDTO { CanteenId = 1 };
            _canteenRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((Canteen?)null);

            // Act
            var result = await _controller.Create(dto);

            // Assert
            var badRequest = result.Result.Should().BeOfType<BadRequestObjectResult>().Subject;
            badRequest.Value.Should().Be("Invalid canteen ID");
        }

        [Fact]
        public async Task Create_WithInvalidProducts_ReturnsBadRequest()
        {
            // Arrange
            SetupUserRole("CanteenStaff", _controller);
            var dto = new CreatePackageDTO
            {
                CanteenId = 1,
                ProductIds = new List<int> { 1, 2 },
            };

            _canteenRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new Canteen());
            _productRepository
                .Setup(r => r.GetProductsByIdsAsync(dto.ProductIds))
                .ReturnsAsync(new List<Product> { new() });

            // Act
            var result = await _controller.Create(dto);

            // Assert
            var badRequest = result.Result.Should().BeOfType<BadRequestObjectResult>().Subject;
            badRequest.Value.Should().Be("One or more product IDs are invalid");
        }

        [Fact]
        public async Task Create_WithValidInput_ReturnsCreatedPackage()
        {
            // Arrange
            SetupUserRole("CanteenStaff", _controller);
            var dto = new CreatePackageDTO
            {
                Name = "Test Package",
                CanteenId = 1,
                ProductIds = new List<int> { 1 },
                Type = MealType.Warm,
                PickupTime = DateTime.Now.AddDays(1),
                Price = 5.95m,
            };

            var canteen = new Canteen { Id = 1 };
            var products = new List<Product> { new() { Id = 1 } };

            _canteenRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(canteen);
            _productRepository
                .Setup(r => r.GetProductsByIdsAsync(dto.ProductIds))
                .ReturnsAsync(products);
            _packageRepository
                .Setup(r => r.CreatePackageAsync(It.IsAny<Package>()))
                .ReturnsAsync(new Package { Id = 1 });

            // Act
            var result = await _controller.Create(dto);

            // Assert
            result.Result.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task Delete_WithNonExistentPackage_ReturnsNotFound()
        {
            // Arrange
            SetupUserRole("CanteenStaff", _controller);
            _packageRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((Package?)null);

            // Act
            var result = await _controller.Delete(1);

            // Assert
            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task Delete_WithReservedPackage_ReturnsBadRequest()
        {
            // Arrange
            SetupUserRole("CanteenStaff", _controller);
            _packageRepository
                .Setup(r => r.GetByIdAsync(1))
                .ReturnsAsync(new Package { ReservedById = "user1" });

            // Act
            var result = await _controller.Delete(1);

            // Assert
            var badRequest = result.Should().BeOfType<BadRequestObjectResult>().Subject;
            badRequest.Value.Should().Be("Cannot delete package that is already reserved");
        }

        [Fact]
        public async Task Delete_WithValidPackage_ReturnsNoContent()
        {
            // Arrange
            SetupUserRole("CanteenStaff", _controller);
            _packageRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new Package());

            // Act
            var result = await _controller.Delete(1);

            // Assert
            result.Should().BeOfType<NoContentResult>();
        }

        [Fact]
        public async Task Update_WithNonExistentPackage_ReturnsNotFound()
        {
            // Arrange
            SetupUserRole("CanteenStaff", _controller);
            _packageRepository
                .Setup(r => r.GetPackageWithProductsAsync(1))
                .ReturnsAsync((Package?)null);

            // Act
            var result = await _controller.Update(1, new CreatePackageDTO());

            // Assert
            result.Result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task Update_WithReservedPackage_ReturnsBadRequest()
        {
            // Arrange
            SetupUserRole("CanteenStaff", _controller);
            _packageRepository
                .Setup(r => r.GetPackageWithProductsAsync(1))
                .ReturnsAsync(new Package { ReservedById = "user1" });

            // Act
            var result = await _controller.Update(1, new CreatePackageDTO());

            // Assert
            var badRequest = result.Result.Should().BeOfType<BadRequestObjectResult>().Subject;
            badRequest.Value.Should().Be("Cannot update package that is already reserved");
        }

        [Fact]
        public async Task Update_WithValidInput_ReturnsUpdatedPackage()
        {
            // Arrange
            SetupUserRole("CanteenStaff", _controller);
            var dto = new CreatePackageDTO
            {
                Name = "Updated Package",
                ProductIds = new List<int> { 1 },
            };

            var package = new Package();
            var products = new List<Product> { new() { Id = 1 } };

            _packageRepository.Setup(r => r.GetPackageWithProductsAsync(1)).ReturnsAsync(package);
            _productRepository
                .Setup(r => r.GetProductsByIdsAsync(dto.ProductIds))
                .ReturnsAsync(products);
            _packageRepository
                .Setup(r => r.UpdatePackageAsync(It.IsAny<Package>()))
                .ReturnsAsync(new Package { Name = dto.Name });

            // Act
            var result = await _controller.Update(1, dto);

            // Assert
            var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
            var updatedPackage = okResult.Value.Should().BeOfType<Package>().Subject;
            updatedPackage.Name.Should().Be(dto.Name);
        }
    }
}

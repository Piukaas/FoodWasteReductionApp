using FluentAssertions;
using FoodWasteReduction.Application.DTOs;
using FoodWasteReduction.Application.Services;
using FoodWasteReduction.Core.Entities;
using FoodWasteReduction.Core.Enums;
using FoodWasteReduction.Core.Interfaces.Repositories;
using Moq;

namespace FoodWasteReduction.Tests.Services
{
    public class PackageServiceTests
    {
        private readonly Mock<IPackageRepository> _packageRepository;
        private readonly Mock<ICanteenRepository> _canteenRepository;
        private readonly Mock<IProductRepository> _productRepository;
        private readonly PackageService _service;

        public PackageServiceTests()
        {
            _packageRepository = new Mock<IPackageRepository>();
            _canteenRepository = new Mock<ICanteenRepository>();
            _productRepository = new Mock<IProductRepository>();
            _service = new PackageService(
                _packageRepository.Object,
                _canteenRepository.Object,
                _productRepository.Object
            );
        }

        [Fact]
        public async Task CreateAsync_ValidInput_ReturnsPackage()
        {
            // Arrange
            var dto = new CreatePackageDTO
            {
                Name = "Test Package",
                CanteenId = 1,
                ProductIds = new List<int> { 1 },
                Type = MealType.Warm,
                PickupTime = DateTime.Now.AddDays(1),
                Price = 5.99m,
                City = City.Breda,
            };

            _canteenRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new Canteen { Id = 1 });

            var products = new List<Product>
            {
                new() { Id = 1, ContainsAlcohol = false },
            };
            _productRepository
                .Setup(r => r.GetProductsByIdsAsync(dto.ProductIds))
                .ReturnsAsync(products);

            var createdPackage = new Package { Id = 1, Name = dto.Name };
            _packageRepository
                .Setup(r => r.CreatePackageAsync(It.IsAny<Package>()))
                .ReturnsAsync(createdPackage);

            // Act
            var (success, package, error) = await _service.CreateAsync(dto);

            // Assert
            success.Should().BeTrue();
            package.Should().NotBeNull();
            error.Should().BeNull();
            package!.Name.Should().Be(dto.Name);
        }

        [Fact]
        public async Task CreateAsync_InvalidCanteen_ReturnsError()
        {
            // Arrange
            var dto = new CreatePackageDTO { CanteenId = 1 };
            _canteenRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((Canteen?)null);

            // Act
            var (success, package, error) = await _service.CreateAsync(dto);

            // Assert
            success.Should().BeFalse();
            package.Should().BeNull();
            error.Should().Be("Invalid canteen ID");
        }

        [Fact]
        public async Task CreateAsync_InvalidProducts_ReturnsError()
        {
            // Arrange
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
            var (success, package, error) = await _service.CreateAsync(dto);

            // Assert
            success.Should().BeFalse();
            package.Should().BeNull();
            error.Should().Be("One or more product IDs are invalid");
        }

        [Fact]
        public async Task DeleteAsync_ValidPackage_ReturnsSuccess()
        {
            // Arrange
            var package = new Package { Id = 1 };
            _packageRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(package);

            // Act
            var (success, error) = await _service.DeleteAsync(1);

            // Assert
            success.Should().BeTrue();
            error.Should().BeNull();
            _packageRepository.Verify(r => r.DeletePackageAsync(package), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_PackageNotFound_ReturnsError()
        {
            // Arrange
            _packageRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((Package?)null);

            // Act
            var (success, error) = await _service.DeleteAsync(1);

            // Assert
            success.Should().BeFalse();
            error.Should().Be("Package not found");
        }

        [Fact]
        public async Task DeleteAsync_ReservedPackage_ReturnsError()
        {
            // Arrange
            var package = new Package { Id = 1, ReservedById = "user1" };
            _packageRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(package);

            // Act
            var (success, error) = await _service.DeleteAsync(1);

            // Assert
            success.Should().BeFalse();
            error.Should().Be("Cannot delete package that is already reserved");
        }

        [Fact]
        public async Task UpdateAsync_ValidInput_ReturnsUpdatedPackage()
        {
            // Arrange
            var dto = new CreatePackageDTO
            {
                Name = "Updated Package",
                ProductIds = new List<int> { 1 },
                Type = MealType.Warm,
                PickupTime = DateTime.Now.AddDays(1),
                Price = 4.99m,
            };

            var package = new Package { Id = 1 };
            var products = new List<Product> { new() { Id = 1 } };

            _packageRepository.Setup(r => r.GetPackageWithProductsAsync(1)).ReturnsAsync(package);
            _productRepository
                .Setup(r => r.GetProductsByIdsAsync(dto.ProductIds))
                .ReturnsAsync(products);
            _packageRepository
                .Setup(r => r.UpdatePackageAsync(It.IsAny<Package>()))
                .ReturnsAsync(new Package { Id = 1, Name = dto.Name });

            // Act
            var (success, updatedPackage, error) = await _service.UpdateAsync(1, dto);

            // Assert
            success.Should().BeTrue();
            updatedPackage.Should().NotBeNull();
            error.Should().BeNull();
            updatedPackage!.Name.Should().Be(dto.Name);
        }

        [Fact]
        public async Task UpdateAsync_PackageNotFound_ReturnsError()
        {
            // Arrange
            _packageRepository
                .Setup(r => r.GetPackageWithProductsAsync(1))
                .ReturnsAsync((Package?)null);

            // Act
            var (success, package, error) = await _service.UpdateAsync(1, new CreatePackageDTO());

            // Assert
            success.Should().BeFalse();
            package.Should().BeNull();
            error.Should().Be("Package not found");
        }

        [Fact]
        public async Task UpdateAsync_ReservedPackage_ReturnsError()
        {
            // Arrange
            var package = new Package { Id = 1, ReservedById = "user1" };
            _packageRepository.Setup(r => r.GetPackageWithProductsAsync(1)).ReturnsAsync(package);

            // Act
            var (success, updatedPackage, error) = await _service.UpdateAsync(
                1,
                new CreatePackageDTO()
            );

            // Assert
            success.Should().BeFalse();
            updatedPackage.Should().BeNull();
            error.Should().Be("Cannot update package that is already reserved");
        }

        [Fact]
        public async Task UpdateAsync_InvalidProducts_ReturnsError()
        {
            // Arrange
            var dto = new CreatePackageDTO { ProductIds = [1, 2] };
            var package = new Package { Id = 1 };

            _packageRepository.Setup(r => r.GetPackageWithProductsAsync(1)).ReturnsAsync(package);
            _productRepository
                .Setup(r => r.GetProductsByIdsAsync(dto.ProductIds))
                .ReturnsAsync(new List<Product> { new() });

            // Act
            var (success, updatedPackage, error) = await _service.UpdateAsync(1, dto);

            // Assert
            success.Should().BeFalse();
            updatedPackage.Should().BeNull();
            error.Should().Be("One or more product IDs are invalid");
        }
    }
}

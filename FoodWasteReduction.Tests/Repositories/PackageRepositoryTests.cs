using FluentAssertions;
using FoodWasteReduction.Api.Repositories;
using FoodWasteReduction.Core.Entities;
using FoodWasteReduction.Core.Enums;
using Microsoft.EntityFrameworkCore;

namespace FoodWasteReduction.Tests.Repositories
{
    public class PackageRepositoryTests : RepositoryTestBase
    {
        private readonly PackageRepository _repository;

        public PackageRepositoryTests()
            : base()
        {
            _repository = new PackageRepository(Context);
        }

        private async Task<Package> CreateTestPackage(bool withProducts = false)
        {
            var package = new Package
            {
                Name = "Test Package",
                Price = 5.95m,
                Type = MealType.Warm,
                PickupTime = DateTime.Now.AddHours(2),
                ExpiryTime = DateTime.Now.AddHours(4),
                Products = withProducts ? [new() { Name = "Test Product" }] : [],
            };

            return await _repository.CreatePackageAsync(package);
        }

        [Fact]
        public async Task GetByIdAsync_NonExistentPackage_ReturnsNull()
        {
            // Act
            var result = await _repository.GetByIdAsync(999);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task GetPackageWithProductsAsync_WithProducts_ReturnsPackageAndProducts()
        {
            // Arrange
            var package = await CreateTestPackage(true);

            // Act
            var result = await _repository.GetPackageWithProductsAsync(package.Id);

            // Assert
            result.Should().NotBeNull();
            result!.Products.Should().NotBeEmpty();
        }

        [Fact]
        public async Task UpdatePackageAsync_ValidPackage_UpdatesPackage()
        {
            // Arrange
            var package = await CreateTestPackage();
            var newName = "Updated Package";
            package.Name = newName;

            // Act
            var result = await _repository.UpdatePackageAsync(package);

            // Assert
            result.Name.Should().Be(newName);
        }

        [Fact]
        public async Task DeletePackageAsync_ExistingPackage_RemovesPackage()
        {
            // Arrange
            var package = await CreateTestPackage();

            // Act
            await _repository.DeletePackageAsync(package);
            var result = await _repository.GetByIdAsync(package.Id);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task HasReservationOnDateAsync_NoReservation_ReturnsFalse()
        {
            // Arrange
            var userId = "test-user-id";
            var date = DateTime.Now;

            // Act
            var result = await _repository.HasReservationOnDateAsync(userId, date);

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void GetPackagesGraphQL_ReturnsQueryable()
        {
            // Act
            var result = _repository.GetPackagesGraphQL();

            // Assert
            result.Should().BeAssignableTo<IQueryable<Package>>();
        }

        [Fact]
        public async Task ReservePackageAsync_AlreadyReserved_UpdatesReservation()
        {
            // Arrange
            var package = await CreateTestPackage();
            var userId1 = "user-1";
            var userId2 = "user-2";

            // Act
            await _repository.ReservePackageAsync(package, userId1);
            var result = await _repository.ReservePackageAsync(package, userId2);

            // Assert
            result.ReservedById.Should().Be(userId2);
        }

        [Fact]
        public async Task GetPackageWithDetailsAsync_WithNoProducts_ReturnsPackageWithoutProducts()
        {
            // Arrange
            var package = await CreateTestPackage();

            // Act
            var result = await _repository.GetPackageWithDetailsAsync(package.Id);

            // Assert
            result.Should().NotBeNull();
            result!.Products.Should().BeEmpty();
        }
    }
}

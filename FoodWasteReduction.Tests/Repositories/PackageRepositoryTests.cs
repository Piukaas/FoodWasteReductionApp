using FluentAssertions;
using FoodWasteReduction.Api.Repositories;
using FoodWasteReduction.Core.Entities;
using FoodWasteReduction.Core.Enums;

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

        private async Task<Package> CreateTestPackage()
        {
            var package = new Package
            {
                Name = "Test Package",
                Price = 5.95m,
                Type = MealType.Warm,
                PickupTime = DateTime.Now.AddHours(2),
            };

            return await _repository.CreatePackageAsync(package);
        }

        [Fact]
        public async Task CreatePackageAsync_ValidPackage_CreatesAndReturnsPackage()
        {
            // Arrange
            var package = new Package
            {
                Name = "New Package",
                Price = 4.95m,
                Type = MealType.Warm,
            };

            // Act
            var result = await _repository.CreatePackageAsync(package);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().BeGreaterThan(0);
            result.Name.Should().Be(package.Name);
        }

        [Fact]
        public async Task GetPackageWithDetailsAsync_ExistingPackage_ReturnsPackageWithProducts()
        {
            // Arrange
            var package = await CreateTestPackage();

            // Act
            var result = await _repository.GetPackageWithDetailsAsync(package.Id);

            // Assert
            result.Should().NotBeNull();
            result!.Id.Should().Be(package.Id);
        }

        [Fact]
        public async Task ReservePackageAsync_ValidPackage_UpdatesReservation()
        {
            // Arrange
            var package = await CreateTestPackage();
            var userId = "test-user-id";

            // Act
            var result = await _repository.ReservePackageAsync(package, userId);

            // Assert
            result.ReservedById.Should().Be(userId);
        }

        [Fact]
        public async Task HasReservationOnDateAsync_WithExistingReservation_ReturnsTrue()
        {
            // Arrange
            var package = await CreateTestPackage();
            var userId = "test-user-id";
            await _repository.ReservePackageAsync(package, userId);

            // Act
            var result = await _repository.HasReservationOnDateAsync(userId, package.PickupTime);

            // Assert
            result.Should().BeTrue();
        }
    }
}

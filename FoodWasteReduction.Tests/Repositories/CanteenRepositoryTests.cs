using FluentAssertions;
using FoodWasteReduction.Core.Entities;
using FoodWasteReduction.Core.Enums;
using FoodWasteReduction.Infrastructure.Repositories;

namespace FoodWasteReduction.Tests.Repositories
{
    public class CanteenRepositoryTests : RepositoryTestBase
    {
        private readonly CanteenRepository _repository;

        public CanteenRepositoryTests()
            : base()
        {
            _repository = new CanteenRepository(Context);
        }

        private async Task<Canteen> CreateTestCanteen()
        {
            var canteen = new Canteen
            {
                City = City.Breda,
                Location = Location.LA,
                ServesWarmMeals = true,
            };

            return await _repository.CreateAsync(canteen);
        }

        [Fact]
        public async Task GetByIdAsync_ExistingCanteen_ReturnsCanteen()
        {
            // Arrange
            var canteen = await CreateTestCanteen();

            // Act
            var result = await _repository.GetByIdAsync(canteen.Id);

            // Assert
            result.Should().NotBeNull();
            result!.Id.Should().Be(canteen.Id);
            result.City.Should().Be(canteen.City);
        }

        [Fact]
        public async Task GetByIdAsync_NonExistingCanteen_ReturnsNull()
        {
            // Act
            var result = await _repository.GetByIdAsync(-1);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task CreateAsync_ValidCanteen_CreatesAndReturnsCanteen()
        {
            // Arrange
            var canteen = new Canteen
            {
                City = City.Tilburg,
                Location = Location.LD,
                ServesWarmMeals = false,
            };

            // Act
            var result = await _repository.CreateAsync(canteen);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().BeGreaterThan(0);
            result.City.Should().Be(canteen.City);
        }

        [Fact]
        public async Task GetAllAsync_ReturnsAllCanteens()
        {
            // Arrange
            var canteen1 = await CreateTestCanteen();
            var canteen2 = await CreateTestCanteen();

            // Act
            var result = await _repository.GetAllAsync();

            // Assert
            result.Should().HaveCount(2);
            result.Should().Contain(c => c.Id == canteen1.Id);
            result.Should().Contain(c => c.Id == canteen2.Id);
        }
    }
}

using FluentAssertions;
using FoodWasteReduction.Core.Entities;
using FoodWasteReduction.Core.Enums;
using FoodWasteReduction.Infrastructure.Repositories;

namespace FoodWasteReduction.Tests.Repositories
{
    public class CanteenStaffRepositoryTests : RepositoryTestBase
    {
        private readonly CanteenStaffRepository _repository;

        public CanteenStaffRepositoryTests()
            : base()
        {
            _repository = new CanteenStaffRepository(Context);
        }

        private async Task<CanteenStaff> CreateTestStaff()
        {
            var staff = new CanteenStaff
            {
                Id = "test-id",
                PersonnelNumber = "P123456",
                Location = Location.LA,
            };

            return await _repository.CreateAsync(staff);
        }

        [Fact]
        public async Task GetByIdAsync_ExistingStaff_ReturnsStaff()
        {
            // Arrange
            var staff = await CreateTestStaff();

            // Act
            var result = await _repository.GetByIdAsync(staff.Id);

            // Assert
            result.Should().NotBeNull();
            result!.Id.Should().Be(staff.Id);
            result.Location.Should().Be(staff.Location);
            result.PersonnelNumber.Should().Be(staff.PersonnelNumber);
        }

        [Fact]
        public async Task GetByIdAsync_NonExistingStaff_ReturnsNull()
        {
            // Act
            var result = await _repository.GetByIdAsync("non-existing-id");

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task CreateAsync_ValidStaff_CreatesAndReturnsStaff()
        {
            // Arrange
            var staff = new CanteenStaff
            {
                Id = "new-id",
                PersonnelNumber = "P654321",
                Location = Location.LD,
            };

            // Act
            var result = await _repository.CreateAsync(staff);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(staff.Id);
        }
    }
}

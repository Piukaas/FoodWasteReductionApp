using FluentAssertions;
using FoodWasteReduction.Core.DTOs;

namespace FoodWasteReduction.Tests.Models.DTOs
{
    public class ReservePackageDTOTests
    {
        private static ReservePackageDTO CreateValidDTO()
        {
            return new ReservePackageDTO { PackageId = 1, UserId = "test-user-id" };
        }

        [Fact]
        public void ReservePackageDTO_DefaultValues_AreCorrect()
        {
            // Arrange & Act
            var dto = new ReservePackageDTO();

            // Assert
            dto.PackageId.Should().Be(0);
            dto.UserId.Should().BeEmpty();
        }

        [Fact]
        public void ReservePackageDTO_PropertiesSetCorrectly()
        {
            // Arrange
            var dto = CreateValidDTO();

            // Assert
            dto.PackageId.Should().Be(1);
            dto.UserId.Should().Be("test-user-id");
        }
    }
}

using System.ComponentModel.DataAnnotations;
using FluentAssertions;
using FoodWasteReduction.Application.DTOs;
using FoodWasteReduction.Core.Enums;

namespace FoodWasteReduction.Tests.Models.DTOs
{
    public class CreatePackageDTOTests
    {
        private static CreatePackageDTO CreateValidPackageDTO()
        {
            return new CreatePackageDTO
            {
                Name = "Test Package",
                City = City.Breda,
                CanteenId = 1,
                Type = MealType.Warm,
                PickupTime = DateTime.Now.AddHours(1),
                Price = 5.95m,
                ProductIds = [1, 2, 3],
            };
        }

        [Fact]
        public void CreatePackageDTO_WithValidData_PassesValidation()
        {
            // Arrange
            var dto = CreateValidPackageDTO();

            // Act
            var context = new ValidationContext(dto);
            var results = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(dto, context, results, true);

            // Assert
            isValid.Should().BeTrue();
            results.Should().BeEmpty();
        }

        [Fact]
        public void CreatePackageDTO_WithMissingRequiredFields_FailsValidation()
        {
            // Arrange
            var dto = new CreatePackageDTO();

            // Act
            var context = new ValidationContext(dto);
            var results = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(dto, context, results, true);

            // Assert
            isValid.Should().BeFalse();
            var memberNames = results.SelectMany(r => r.MemberNames).ToList();
            memberNames.Should().BeEquivalentTo(["Name", "PickupTime"]);
        }

        [Theory]
        [InlineData(-1)]
        public void CreatePackageDTO_WithInvalidPrice_FailsValidation(decimal price)
        {
            // Arrange
            var dto = CreateValidPackageDTO();
            dto.Price = price;

            // Act
            var context = new ValidationContext(dto);
            var results = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(dto, context, results, true);

            // Assert
            isValid.Should().BeFalse();
            results.Should().Contain(r => r.MemberNames.Contains("Price"));
        }

        [Fact]
        public void CreatePackageDTO_WithPastPickupTime_FailsValidation()
        {
            // Arrange
            var dto = CreateValidPackageDTO();
            dto.PickupTime = DateTime.Now.AddHours(-1);

            // Act
            var context = new ValidationContext(dto);
            var results = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(dto, context, results, true);

            // Assert
            isValid.Should().BeFalse();
            results.Should().Contain(r => r.MemberNames.Contains("PickupTime"));
        }
    }
}

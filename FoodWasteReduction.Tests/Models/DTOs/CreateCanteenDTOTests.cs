using System.ComponentModel.DataAnnotations;
using FluentAssertions;
using FoodWasteReduction.Application.DTOs;
using FoodWasteReduction.Core.Enums;

namespace FoodWasteReduction.Tests.Models.DTOs
{
    public class CreateCanteenDTOTests
    {
        private static CreateCanteenDTO CreateValidCanteenDTO()
        {
            return new CreateCanteenDTO
            {
                City = City.Breda,
                Location = Location.LA,
                ServesWarmMeals = true,
            };
        }

        [Fact]
        public void CreateCanteenDTO_WithValidData_PassesValidation()
        {
            // Arrange
            var dto = CreateValidCanteenDTO();

            // Act
            var context = new ValidationContext(dto);
            var results = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(dto, context, results, true);

            // Assert
            isValid.Should().BeTrue();
            results.Should().BeEmpty();
        }

        [Fact]
        public void CreateCanteenDTO_DefaultValues_AreCorrect()
        {
            // Arrange & Act
            var dto = new CreateCanteenDTO();

            // Assert
            dto.ServesWarmMeals.Should().BeFalse();
            dto.City.Should().Be(default);
            dto.Location.Should().Be(default);
        }
    }
}

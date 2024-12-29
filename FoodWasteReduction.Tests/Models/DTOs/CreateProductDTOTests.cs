using System.ComponentModel.DataAnnotations;
using FluentAssertions;
using FoodWasteReduction.Core.DTOs;

namespace FoodWasteReduction.Tests.Models.DTOs
{
    public class CreateProductDTOTests
    {
        private static CreateProductDTO CreateValidProductDTO()
        {
            return new CreateProductDTO
            {
                Name = "Test Product",
                ContainsAlcohol = false,
                ImageUrl = "https://example.com/image.jpg",
            };
        }

        [Fact]
        public void CreateProductDTO_WithValidData_PassesValidation()
        {
            // Arrange
            var dto = CreateValidProductDTO();

            // Act
            var context = new ValidationContext(dto);
            var results = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(dto, context, results, true);

            // Assert
            isValid.Should().BeTrue();
            results.Should().BeEmpty();
        }

        [Fact]
        public void CreateProductDTO_WithMissingRequiredFields_FailsValidation()
        {
            // Arrange
            var dto = new CreateProductDTO();

            // Act
            var context = new ValidationContext(dto);
            var results = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(dto, context, results, true);

            // Assert
            isValid.Should().BeFalse();
            results.Should().Contain(r => r.MemberNames.Contains("Name"));
        }

        [Fact]
        public void CreateProductDTO_DefaultValues_AreCorrect()
        {
            // Arrange & Act
            var dto = new CreateProductDTO();

            // Assert
            dto.Name.Should().BeEmpty();
            dto.ContainsAlcohol.Should().BeFalse();
            dto.ImageUrl.Should().BeNull();
        }
    }
}

using System.ComponentModel.DataAnnotations;
using FluentAssertions;
using FoodWasteReduction.Web.Models;

namespace FoodWasteReduction.Tests.Models.ViewModels
{
    public class ProductViewModelTests
    {
        private static ProductViewModel CreateValidProduct()
        {
            return new ProductViewModel
            {
                Name = "Test Product",
                ContainsAlcohol = false,
                ImageUrl = "https://example.com/image.jpg",
            };
        }

        [Fact]
        public void ProductViewModel_WithValidData_PassesValidation()
        {
            // Arrange
            var model = CreateValidProduct();

            // Act
            var context = new ValidationContext(model);
            var results = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(model, context, results, true);

            // Assert
            isValid.Should().BeTrue();
            results.Should().BeEmpty();
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void ProductViewModel_WithInvalidName_FailsValidation(string name)
        {
            // Arrange
            var model = CreateValidProduct();
            model.Name = name;

            // Act
            var context = new ValidationContext(model);
            var results = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(model, context, results, true);

            // Assert
            isValid.Should().BeFalse();
            results.Should().Contain(r => r.ErrorMessage == "Naam is verplicht");
        }

        [Theory]
        [InlineData("not-a-url")]
        public void ProductViewModel_WithInvalidImageUrl_FailsValidation(string url)
        {
            // Arrange
            var model = CreateValidProduct();
            model.ImageUrl = url;

            // Act
            var context = new ValidationContext(model);
            var results = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(model, context, results, true);

            // Assert
            isValid.Should().BeFalse();
            results.Should().Contain(r => r.ErrorMessage == "Voer een geldige URL in");
        }
    }
}

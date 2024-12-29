using System.ComponentModel.DataAnnotations;
using FluentAssertions;
using FoodWasteReduction.Core.Entities;

namespace FoodWasteReduction.Tests.Models.Entities
{
    public class ProductTests
    {
        private static Product CreateValidProduct()
        {
            return new Product
            {
                Id = 1,
                Name = "Test Product",
                ContainsAlcohol = false,
                ImageUrl = "https://example.com/image.jpg",
            };
        }

        [Fact]
        public void Product_WithValidData_PassesValidation()
        {
            // Arrange
            var product = CreateValidProduct();

            // Act
            var context = new ValidationContext(product);
            var results = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(product, context, results, true);

            // Assert
            isValid.Should().BeTrue();
            results.Should().BeEmpty();
        }

        [Fact]
        public void Product_WithMissingRequiredFields_FailsValidation()
        {
            // Arrange
            var product = new Product();

            // Act
            var context = new ValidationContext(product);
            var results = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(
                product,
                context,
                results,
                validateAllProperties: true
            );

            // Assert
            isValid.Should().BeFalse();
            var memberNames = results.SelectMany(r => r.MemberNames).ToList();
            memberNames.Should().BeEquivalentTo(["Name"]);
        }

        [Fact]
        public void Product_DefaultValues_AreCorrect()
        {
            // Arrange & Act
            var product = new Product();

            // Assert
            product.Id.Should().Be(0);
            product.Name.Should().BeEmpty();
            product.ContainsAlcohol.Should().BeFalse();
            product.ImageUrl.Should().BeNull();
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void Product_ContainsAlcohol_SetsCorrectly(bool containsAlcohol)
        {
            // Arrange
            var product = CreateValidProduct();

            // Act
            product.ContainsAlcohol = containsAlcohol;

            // Assert
            product.ContainsAlcohol.Should().Be(containsAlcohol);
        }
    }
}

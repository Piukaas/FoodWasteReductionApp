using System.ComponentModel.DataAnnotations;
using FluentAssertions;
using FoodWasteReduction.Core.Enums;
using FoodWasteReduction.Web.Models;

namespace FoodWasteReduction.Tests.Models.ViewModels
{
    public class PackageViewModelTests
    {
        private static PackageViewModel CreateValidPackage()
        {
            return new PackageViewModel
            {
                Name = "Test Package",
                Type = MealType.Warm,
                PickupTime = DateTime.Now.AddHours(1),
                ExpiryTime = DateTime.Now.AddHours(3),
                Price = 5.95m,
                ProductIds = [1, 2],
            };
        }

        [Fact]
        public void PackageViewModel_WithValidData_PassesValidation()
        {
            // Arrange
            var model = CreateValidPackage();

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
        public void PackageViewModel_WithInvalidName_FailsValidation(string name)
        {
            // Arrange
            var model = CreateValidPackage();
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
        [InlineData(-1)]
        [InlineData(-0.01)]
        public void PackageViewModel_WithInvalidPrice_FailsValidation(decimal price)
        {
            // Arrange
            var model = CreateValidPackage();
            model.Price = price;

            // Act
            var context = new ValidationContext(model);
            var results = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(model, context, results, true);

            // Assert
            isValid.Should().BeFalse();
            results.Should().Contain(r => r.ErrorMessage == "Prijs moet positief zijn");
        }

        [Fact]
        public void PackageViewModel_WithFarFuturePickupTime_FailsValidation()
        {
            // Arrange
            var model = CreateValidPackage();
            model.PickupTime = DateTime.Now.AddDays(3);

            // Act
            var context = new ValidationContext(model);
            var results = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(model, context, results, true);

            // Assert
            isValid.Should().BeFalse();
            results
                .Should()
                .Contain(r => r.ErrorMessage == "Ophaaltijd mag maximaal 2 dagen vooruit liggen");
        }
    }
}

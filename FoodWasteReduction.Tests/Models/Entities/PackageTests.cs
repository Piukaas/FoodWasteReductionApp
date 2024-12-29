using System.ComponentModel.DataAnnotations;
using FluentAssertions;
using FoodWasteReduction.Core.Entities;
using FoodWasteReduction.Core.Enums;

namespace FoodWasteReduction.Tests.Models.Entities
{
    public class PackageTests
    {
        private static Package CreateValidPackage()
        {
            return new Package
            {
                Name = "Test Package",
                City = City.Breda,
                CanteenId = 1,
                Type = MealType.Warm,
                PickupTime = DateTime.Now.AddHours(1),
                ExpiryTime = DateTime.Now.AddHours(2),
                Price = 5.95m,
                Is18Plus = false,
            };
        }

        [Fact]
        public void Package_WithValidData_PassesValidation()
        {
            // Arrange
            var package = CreateValidPackage();

            // Act
            var context = new ValidationContext(package);
            var results = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(package, context, results, true);

            // Assert
            isValid.Should().BeTrue();
            results.Should().BeEmpty();
        }

        [Fact]
        public void Package_WithMissingRequiredFields_FailsValidation()
        {
            // Arrange
            var package = new Package();

            // Act
            var context = new ValidationContext(package);
            var results = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(
                package,
                context,
                results,
                validateAllProperties: true
            );

            // Assert
            isValid.Should().BeFalse();
            var memberNames = results.SelectMany(r => r.MemberNames).ToList();
            memberNames.Should().BeEquivalentTo(["Name", "PickupTime"]);
        }

        [Fact]
        public void Package_DefaultValues_AreCorrect()
        {
            // Arrange & Act
            var package = new Package();

            // Assert
            package.Id.Should().Be(0);
            package.Name.Should().BeEmpty();
            package.Is18Plus.Should().BeFalse();
            package.ReservedById.Should().BeNull();
            package.Products.Should().BeEmpty();
        }

        [Theory]
        [InlineData(-1)]
        public void Package_WithInvalidPrice_FailsValidation(decimal price)
        {
            // Arrange
            var package = CreateValidPackage();
            package.Price = price;

            // Act
            var context = new ValidationContext(package);
            var results = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(package, context, results, true);

            // Assert
            isValid.Should().BeFalse();
        }

        [Fact]
        public void Package_WithFutureDateForPickupTime_FailsValidation()
        {
            // Arrange
            var package = CreateValidPackage();
            package.PickupTime = DateTime.Now.AddDays(3);
            package.ExpiryTime = DateTime.Now.AddDays(3).AddHours(2);

            // Act
            var context = new ValidationContext(package);
            var results = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(package, context, results, true);

            // Assert
            isValid.Should().BeFalse();
        }
    }
}

using System.ComponentModel.DataAnnotations;
using FluentAssertions;
using FoodWasteReduction.Core.Entities;
using FoodWasteReduction.Core.Enums;

namespace FoodWasteReduction.Tests.Models.Entities
{
    public class CanteenTests
    {
        private static Canteen CreateValidCanteen()
        {
            return new Canteen
            {
                City = City.Breda,
                Location = Location.LA,
                ServesWarmMeals = true,
            };
        }

        [Fact]
        public void Canteen_WithValidData_PassesValidation()
        {
            // Arrange
            var canteen = CreateValidCanteen();

            // Act
            var context = new ValidationContext(canteen);
            var results = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(canteen, context, results, true);

            // Assert
            isValid.Should().BeTrue();
            results.Should().BeEmpty();
        }

        [Fact]
        public void Canteen_WithMissingRequiredFields_ChangeToDefault()
        {
            // Arrange
            var canteen = new Canteen();

            // Assert
            canteen.City.Should().Be(default);
            canteen.Location.Should().Be(default);
            ((int)canteen.City).Should().Be(0);
            ((int)canteen.Location).Should().Be(0);
        }

        [Fact]
        public void Canteen_DefaultValues_AreCorrect()
        {
            // Arrange & Act
            var canteen = new Canteen();

            // Assert
            canteen.Id.Should().Be(0);
            canteen.ServesWarmMeals.Should().BeFalse();
        }

        [Theory]
        [InlineData(City.Breda)]
        [InlineData(City.Tilburg)]
        public void Canteen_WithValidCity_PassesValidation(City city)
        {
            // Arrange
            var canteen = CreateValidCanteen();
            canteen.City = city;

            // Act
            var context = new ValidationContext(canteen);
            var results = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(canteen, context, results, true);

            // Assert
            isValid.Should().BeTrue();
            results.Should().BeEmpty();
        }

        [Theory]
        [InlineData(Location.LA)]
        [InlineData(Location.LD)]
        [InlineData(Location.HB)]
        [InlineData(Location.HS)]
        public void Canteen_WithValidLocation_PassesValidation(Location location)
        {
            // Arrange
            var canteen = CreateValidCanteen();
            canteen.Location = location;

            // Act
            var context = new ValidationContext(canteen);
            var results = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(canteen, context, results, true);

            // Assert
            isValid.Should().BeTrue();
            results.Should().BeEmpty();
        }
    }
}

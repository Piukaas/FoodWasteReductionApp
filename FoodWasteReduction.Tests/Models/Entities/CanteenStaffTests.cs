using System.ComponentModel.DataAnnotations;
using FluentAssertions;
using FoodWasteReduction.Core.Entities;
using FoodWasteReduction.Core.Enums;

namespace FoodWasteReduction.Tests.Models.Entities
{
    public class CanteenStaffTests
    {
        private static CanteenStaff CreateValidStaff()
        {
            return new CanteenStaff
            {
                Id = "test-id",
                PersonnelNumber = "P123456",
                Location = Location.LA,
            };
        }

        [Fact]
        public void CanteenStaff_WithValidData_PassesValidation()
        {
            // Arrange
            var staff = CreateValidStaff();

            // Act
            var context = new ValidationContext(staff);
            var results = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(staff, context, results, true);

            // Assert
            isValid.Should().BeTrue();
            results.Should().BeEmpty();
        }

        [Fact]
        public void CanteenStaff_WithMissingRequiredFields_FailsValidation()
        {
            // Arrange
            var staff = new CanteenStaff();

            // Act
            var context = new ValidationContext(staff);
            var results = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(
                staff,
                context,
                results,
                validateAllProperties: true
            );

            // Assert
            isValid.Should().BeFalse();
            var memberNames = results.SelectMany(r => r.MemberNames).ToList();
            memberNames.Should().BeEquivalentTo(["PersonnelNumber"]);
        }

        [Fact]
        public void CanteenStaff_DefaultValues_AreCorrect()
        {
            // Arrange & Act
            var staff = new CanteenStaff();

            // Assert
            staff.Id.Should().BeEmpty();
            staff.PersonnelNumber.Should().BeEmpty();
            staff.Location.Should().Be(default);
        }

        [Theory]
        [InlineData(Location.LA)]
        [InlineData(Location.LD)]
        [InlineData(Location.HS)]
        [InlineData(Location.TA)]
        public void CanteenStaff_WithValidLocation_PassesValidation(Location location)
        {
            // Arrange
            var staff = CreateValidStaff();
            staff.Location = location;

            // Act
            var context = new ValidationContext(staff);
            var results = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(staff, context, results, true);

            // Assert
            isValid.Should().BeTrue();
            results.Should().BeEmpty();
        }
    }
}

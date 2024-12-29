using System.ComponentModel.DataAnnotations;
using FluentAssertions;
using FoodWasteReduction.Core.Enums;
using FoodWasteReduction.Web.Models.Auth;

namespace FoodWasteReduction.Tests.Models.ViewModels.Auth
{
    public class RegisterCanteenStaffViewModelTests
    {
        private static RegisterCanteenStaffViewModel CreateValidModel()
        {
            return new RegisterCanteenStaffViewModel
            {
                Email = "staff@example.com",
                Name = "John Doe",
                PersonnelNumber = "P123456",
                Location = Location.LA,
                Password = "Password123!",
                ConfirmPassword = "Password123!",
            };
        }

        [Fact]
        public void RegisterCanteenStaffViewModel_WithValidData_PassesValidation()
        {
            // Arrange
            var model = CreateValidModel();

            // Act
            var context = new ValidationContext(model);
            var results = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(model, context, results, true);

            // Assert
            isValid.Should().BeTrue();
            results.Should().BeEmpty();
        }

        [Fact]
        public void RegisterCanteenStaffViewModel_WithMismatchedPasswords_FailsValidation()
        {
            // Arrange
            var model = CreateValidModel();
            model.ConfirmPassword = "DifferentPassword123!";

            // Act
            var context = new ValidationContext(model);
            var results = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(model, context, results, true);

            // Assert
            isValid.Should().BeFalse();
            results.Should().Contain(r => r.ErrorMessage == "Wachtwoorden komen niet overeen");
        }

        [Fact]
        public void RegisterCanteenStaffViewModel_WithShortPassword_FailsValidation()
        {
            // Arrange
            var model = CreateValidModel();
            model.Password = "short";
            model.ConfirmPassword = "short";

            // Act
            var context = new ValidationContext(model);
            var results = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(model, context, results, true);

            // Assert
            isValid.Should().BeFalse();
            results
                .Should()
                .Contain(r => r.ErrorMessage == "Wachtwoord moet tenminste 8 karakters bevatten");
        }
    }
}

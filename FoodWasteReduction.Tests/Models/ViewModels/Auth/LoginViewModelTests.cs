using System.ComponentModel.DataAnnotations;
using FluentAssertions;
using FoodWasteReduction.Web.Models.Auth;

namespace FoodWasteReduction.Tests.Models.ViewModels.Auth
{
    public class LoginViewModelTests
    {
        private static LoginViewModel CreateValidModel()
        {
            return new LoginViewModel { Email = "test@example.com", Password = "Password123!" };
        }

        [Fact]
        public void LoginViewModel_WithValidData_PassesValidation()
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

        [Theory]
        [InlineData("", "E-mail is verplicht")]
        [InlineData("invalid-email", "Ongeldig e-mailadres")]
        public void LoginViewModel_WithInvalidEmail_FailsWithCorrectMessage(
            string email,
            string expectedError
        )
        {
            // Arrange
            var model = CreateValidModel();
            model.Email = email;

            // Act
            var context = new ValidationContext(model);
            var results = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(model, context, results, true);

            // Assert
            isValid.Should().BeFalse();
            results.Should().Contain(r => r.ErrorMessage == expectedError);
        }
    }
}

using System.ComponentModel.DataAnnotations;
using FluentAssertions;
using FoodWasteReduction.Core.Enums;
using FoodWasteReduction.Web.Models.Auth;

namespace FoodWasteReduction.Tests.Models.ViewModels.Auth
{
    public class RegisterStudentViewModelTests
    {
        private static RegisterStudentViewModel CreateValidModel()
        {
            return new RegisterStudentViewModel
            {
                Email = "student@example.com",
                Name = "Jane Doe",
                StudentNumber = "S123456",
                DateOfBirth = DateTime.Today.AddYears(-20),
                StudyCity = City.Breda,
                PhoneNumber = "0612345678",
                Password = "Password123!",
                ConfirmPassword = "Password123!",
            };
        }

        [Fact]
        public void RegisterStudentViewModel_WithValidData_PassesValidation()
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
        public void RegisterStudentViewModel_WithInvalidAge_FailsValidation()
        {
            // Arrange
            var model = CreateValidModel();
            model.DateOfBirth = DateTime.Today.AddYears(-15);

            // Act
            var context = new ValidationContext(model);
            var results = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(model, context, results, true);

            // Assert
            isValid.Should().BeFalse();
            results.Should().Contain(r => r.ErrorMessage!.Contains("16 jaar"));
        }

        [Fact]
        public void RegisterStudentViewModel_WithMismatchedPasswords_FailsValidation()
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
            results
                .Should()
                .Contain(r => r.ErrorMessage!.Contains("Wachtwoorden komen niet overeen"));
        }

        [Theory]
        [InlineData("", "Email")]
        [InlineData("invalid-email", "Email")]
        [InlineData("", "Name")]
        [InlineData("", "StudentNumber")]
        [InlineData("", "Password")]
        [InlineData("short", "Password")]
        public void RegisterStudentViewModel_WithInvalidData_FailsValidation(
            string value,
            string propertyName
        )
        {
            // Arrange
            var model = CreateValidModel();
            typeof(RegisterStudentViewModel).GetProperty(propertyName)!.SetValue(model, value);

            // Act
            var context = new ValidationContext(model);
            var results = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(model, context, results, true);

            // Assert
            isValid.Should().BeFalse();
            results.Should().Contain(r => r.MemberNames.Contains(propertyName));
        }

        [Theory]
        [InlineData("0612345678")] // Valid Dutch mobile
        [InlineData("0201234567")] // Valid Dutch landline
        public void RegisterStudentViewModel_WithValidPhoneNumber_PassesValidation(
            string phoneNumber
        )
        {
            // Arrange
            var model = CreateValidModel();
            model.PhoneNumber = phoneNumber;

            // Act
            var context = new ValidationContext(model);
            var results = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(model, context, results, true);

            // Assert
            isValid.Should().BeTrue();
            results.Should().BeEmpty();
        }
    }
}

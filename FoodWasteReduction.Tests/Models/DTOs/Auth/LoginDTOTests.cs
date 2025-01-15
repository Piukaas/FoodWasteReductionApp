using System.ComponentModel.DataAnnotations;
using FluentAssertions;
using FoodWasteReduction.Application.DTOs.Auth;

namespace FoodWasteReduction.Tests.Models.DTOs.Auth
{
    public class LoginDTOTests
    {
        private static LoginDTO CreateValidLoginDTO()
        {
            return new LoginDTO { Email = "test@example.com", Password = "Password123!" };
        }

        [Fact]
        public void LoginDTO_WithValidData_PassesValidation()
        {
            // Arrange
            var dto = CreateValidLoginDTO();

            // Act
            var context = new ValidationContext(dto);
            var results = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(dto, context, results, true);

            // Assert
            isValid.Should().BeTrue();
            results.Should().BeEmpty();
        }

        [Theory]
        [InlineData("")]
        [InlineData("not-an-email")]
        public void LoginDTO_WithInvalidEmail_FailsValidation(string email)
        {
            // Arrange
            var dto = CreateValidLoginDTO();
            dto.Email = email;

            // Act
            var context = new ValidationContext(dto);
            var results = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(dto, context, results, true);

            // Assert
            isValid.Should().BeFalse();
            results.Should().Contain(r => r.MemberNames.Contains("Email"));
        }
    }
}

using System.ComponentModel.DataAnnotations;
using FluentAssertions;
using FoodWasteReduction.Core.DTOs.Auth;
using FoodWasteReduction.Core.Enums;

namespace FoodWasteReduction.Tests.Models.DTOs.Auth
{
    public class RegisterStudentDTOTests
    {
        private static RegisterStudentDTO CreateValidStudentDTO()
        {
            return new RegisterStudentDTO
            {
                Email = "student@example.com",
                Name = "John Doe",
                StudentNumber = "S123456",
                DateOfBirth = DateTime.Today.AddYears(-20),
                StudyCity = City.Breda,
                Password = "Password123!",
                PhoneNumber = "1234567890",
            };
        }

        [Fact]
        public void RegisterStudentDTO_WithValidData_PassesValidation()
        {
            // Arrange
            var dto = CreateValidStudentDTO();

            // Act
            var context = new ValidationContext(dto);
            var results = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(dto, context, results, true);

            // Assert
            isValid.Should().BeTrue();
            results.Should().BeEmpty();
        }

        [Fact]
        public void RegisterStudentDTO_WithInvalidAge_FailsValidation()
        {
            // Arrange
            var dto = CreateValidStudentDTO();
            dto.DateOfBirth = DateTime.Today.AddYears(-15);

            // Act
            var context = new ValidationContext(dto);
            var results = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(dto, context, results, true);

            // Assert
            isValid.Should().BeFalse();
            results.Should().Contain(r => r.ErrorMessage!.Contains("16 jaar"));
        }
    }
}

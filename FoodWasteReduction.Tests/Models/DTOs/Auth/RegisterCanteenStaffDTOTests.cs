using System.ComponentModel.DataAnnotations;
using FluentAssertions;
using FoodWasteReduction.Application.DTOs.Auth;
using FoodWasteReduction.Core.Enums;

namespace FoodWasteReduction.Tests.Models.DTOs.Auth
{
    public class RegisterCanteenStaffDTOTests
    {
        private static RegisterCanteenStaffDTO CreateValidStaffDTO()
        {
            return new RegisterCanteenStaffDTO
            {
                Email = "staff@example.com",
                Name = "Jane Doe",
                PersonnelNumber = "P123456",
                Location = Location.LA,
                Password = "Password123!",
            };
        }

        [Fact]
        public void RegisterCanteenStaffDTO_WithValidData_PassesValidation()
        {
            // Arrange
            var dto = CreateValidStaffDTO();

            // Act
            var context = new ValidationContext(dto);
            var results = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(dto, context, results, true);

            // Assert
            isValid.Should().BeTrue();
            results.Should().BeEmpty();
        }

        [Fact]
        public void RegisterCanteenStaffDTO_WithMissingRequiredFields_FailsValidation()
        {
            // Arrange
            var dto = new RegisterCanteenStaffDTO();

            // Act
            var context = new ValidationContext(dto);
            var results = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(dto, context, results, true);

            // Assert
            isValid.Should().BeFalse();
            var memberNames = results.SelectMany(r => r.MemberNames).ToList();
            memberNames.Should().Contain("Email");
            memberNames.Should().Contain("Name");
            memberNames.Should().Contain("PersonnelNumber");
            memberNames.Should().Contain("Password");
        }
    }
}

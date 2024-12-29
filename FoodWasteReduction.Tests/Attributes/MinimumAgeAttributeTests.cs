using System.ComponentModel.DataAnnotations;
using FluentAssertions;
using FoodWasteReduction.Core.Validations;

namespace FoodWasteReduction.Tests.Attributes
{
    public class MinimumAgeAttributeTests
    {
        private readonly MinimumAgeAttribute _attribute;
        private readonly ValidationContext _validationContext;

        public MinimumAgeAttributeTests()
        {
            _attribute = new MinimumAgeAttribute(16);
            _validationContext = new ValidationContext(new object());
        }

        [Fact]
        public void IsValid_WithNullValue_ReturnsSuccess()
        {
            var result = _attribute.GetValidationResult(null, _validationContext);
            result.Should().Be(ValidationResult.Success);
        }

        [Fact]
        public void IsValid_WithFutureDate_ReturnsError()
        {
            var futureDate = DateTime.Today.AddDays(1);
            var result = _attribute.GetValidationResult(futureDate, _validationContext);

            result.Should().NotBeNull();
            result!.ErrorMessage.Should().Contain("toekomst");
        }

        [Fact]
        public void IsValid_WithAgeBelowMinimum_ReturnsError()
        {
            var dateOfBirth = DateTime.Today.AddYears(-15);
            var result = _attribute.GetValidationResult(dateOfBirth, _validationContext);

            result.Should().NotBeNull();
            result!.ErrorMessage.Should().Contain("16 jaar");
        }

        [Fact]
        public void IsValid_WithAgeEqualToMinimum_ReturnsSuccess()
        {
            var dateOfBirth = DateTime.Today.AddYears(-16);
            var result = _attribute.GetValidationResult(dateOfBirth, _validationContext);

            result.Should().Be(ValidationResult.Success);
        }

        [Fact]
        public void IsValid_WithAgeAboveMinimum_ReturnsSuccess()
        {
            var dateOfBirth = DateTime.Today.AddYears(-20);
            var result = _attribute.GetValidationResult(dateOfBirth, _validationContext);

            result.Should().Be(ValidationResult.Success);
        }

        [Fact]
        public void IsValid_WithBirthdayToday_CalculatesCorrectAge()
        {
            var dateOfBirth = DateTime.Today.AddYears(-16);
            var result = _attribute.GetValidationResult(dateOfBirth, _validationContext);

            result.Should().Be(ValidationResult.Success);
        }

        [Fact]
        public void IsValid_WithBirthdayTomorrow_CalculatesCorrectAge()
        {
            var dateOfBirth = DateTime.Today.AddYears(-16).AddDays(1);
            var result = _attribute.GetValidationResult(dateOfBirth, _validationContext);

            result.Should().NotBeNull();
            result!.ErrorMessage.Should().Contain("16 jaar");
        }
    }
}

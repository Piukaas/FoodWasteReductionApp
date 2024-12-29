using FluentAssertions;
using FoodWasteReduction.Core.Validations;

namespace FoodWasteReduction.Tests.Attributes
{
    public class FutureDateAttributeTests
    {
        private readonly FutureDateAttribute _attribute;

        public FutureDateAttributeTests()
        {
            _attribute = new FutureDateAttribute();
        }

        [Fact]
        public void IsValid_WithNullValue_ReturnsFalse()
        {
            _attribute.IsValid(null).Should().BeFalse();
        }

        [Fact]
        public void IsValid_WithPastDate_ReturnsFalse()
        {
            var pastDate = DateTime.Now.AddDays(-1);
            _attribute.IsValid(pastDate).Should().BeFalse();
        }

        [Fact]
        public void IsValid_WithCurrentDate_ReturnsFalse()
        {
            _attribute.IsValid(DateTime.Now).Should().BeFalse();
        }

        [Fact]
        public void IsValid_WithFutureDateWithinTwoDays_ReturnsTrue()
        {
            var futureDate = DateTime.Now.AddDays(1);
            _attribute.IsValid(futureDate).Should().BeTrue();
        }

        [Fact]
        public void IsValid_WithFutureDateBeyondTwoDays_ReturnsFalse()
        {
            var farFutureDate = DateTime.Now.AddDays(3);
            _attribute.IsValid(farFutureDate).Should().BeFalse();
        }

        [Fact]
        public void IsValid_WithNonDateTimeValue_ReturnsFalse()
        {
            _attribute.IsValid("not a date").Should().BeFalse();
        }
    }
}

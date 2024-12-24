using System;
using Xunit;
using FoodWasteReduction.Core.Validation;

namespace FoodWasteReduction.Tests
{
    public class MinimumAgeAttributeTests
    {
        [Theory]
        [InlineData(18, "2000-01-01", true)]  // Person is over 18
        [InlineData(18, "2010-01-01", false)] // Person is under 18
        [InlineData(18, "2050-01-01", false)] // Future date
        public void MinimumAge_Validation_Works_Correctly(int minimumAge, string dateString, bool shouldBeValid)
        {
            // Arrange
            var attribute = new MinimumAgeAttribute(minimumAge);
            var dateOfBirth = DateTime.Parse(dateString);

            // Act
            var result = attribute.IsValid(dateOfBirth);

            // Assert
            Assert.Equal(shouldBeValid, result);
        }
    }
}
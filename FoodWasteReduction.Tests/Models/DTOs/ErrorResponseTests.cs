using FluentAssertions;
using FoodWasteReduction.Application.DTOs;

namespace FoodWasteReduction.Tests.Models.DTOs
{
    public class ErrorResponseTests
    {
        [Fact]
        public void ErrorResponse_DefaultValues_AreCorrect()
        {
            // Arrange & Act
            var response = new ErrorResponse();

            // Assert
            response.Code.Should().BeEmpty();
            response.Message.Should().BeEmpty();
        }

        [Fact]
        public void ErrorResponse_PropertiesSetCorrectly()
        {
            // Arrange
            var response = new ErrorResponse { Code = "NOT_FOUND", Message = "Resource not found" };

            // Assert
            response.Code.Should().Be("NOT_FOUND");
            response.Message.Should().Be("Resource not found");
        }
    }
}

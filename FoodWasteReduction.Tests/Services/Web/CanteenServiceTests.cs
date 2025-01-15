using System.Net;
using FluentAssertions;
using FoodWasteReduction.Application.DTOs;
using FoodWasteReduction.Core.Entities;
using FoodWasteReduction.Core.Enums;
using FoodWasteReduction.Web.Services;

namespace FoodWasteReduction.Tests.Services.Web
{
    public class CanteenServiceTests : ServiceTestBase
    {
        private readonly CanteenService _service;

        public CanteenServiceTests()
        {
            _service = new CanteenService(HttpClientFactory.Object);
        }

        [Fact]
        public async Task GetCanteens_WhenSuccessful_ReturnsCanteens()
        {
            // Arrange
            var canteens = new List<CanteenDTO>
            {
                new(new Canteen { Id = 1, Location = Location.LA }),
                new(new Canteen { Id = 2, Location = Location.LD }),
            };
            SetupHttpResponse(HttpStatusCode.OK, canteens);

            // Act
            var result = await _service.GetCanteens();

            // Assert
            result.Should().HaveCount(2);
            result.Should().ContainEquivalentOf(canteens[0]);
        }

        [Fact]
        public async Task GetCanteens_WhenError_ReturnsEmptyList()
        {
            // Arrange
            SetupHttpResponse(HttpStatusCode.InternalServerError);

            // Act
            var result = await _service.GetCanteens();

            // Assert
            result.Should().BeEmpty();
        }
    }
}

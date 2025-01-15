using FluentAssertions;
using FoodWasteReduction.Api.Controllers;
using FoodWasteReduction.Application.DTOs;
using FoodWasteReduction.Application.Services.Interfaces;
using FoodWasteReduction.Core.Entities;
using FoodWasteReduction.Core.Enums;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace FoodWasteReduction.Tests.Controllers.Api
{
    public class CanteenControllerTests : ApiControllerTestBase
    {
        private readonly Mock<ICanteenService> _mockCanteenService;
        private readonly CanteensController _controller;

        public CanteenControllerTests()
        {
            _mockCanteenService = new Mock<ICanteenService>();
            _controller = new CanteensController(_mockCanteenService.Object);
            SetupController(_controller);
        }

        [Fact]
        public async Task Create_WithAuthorizedUser_ReturnsOkResult()
        {
            // Arrange
            var dto = new CreateCanteenDTO
            {
                City = City.Breda,
                Location = Location.LA,
                ServesWarmMeals = true,
            };

            var canteen = new Canteen
            {
                Id = 1,
                City = City.Breda,
                Location = Location.LA,
                ServesWarmMeals = true,
            };

            var expectedCanteen = new CanteenDTO(canteen);

            SetupUserRole("CanteenStaff", _controller);
            _mockCanteenService
                .Setup(x => x.CreateAsync(dto))
                .ReturnsAsync((true, expectedCanteen, null));

            // Act
            var result = await _controller.Create(dto);

            // Assert
            var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
            okResult.Value.Should().BeEquivalentTo(expectedCanteen);
            _mockCanteenService.Verify(x => x.CreateAsync(dto), Times.Once);
        }

        [Fact]
        public async Task Create_WithUnauthorizedUser_ReturnsForbidResult()
        {
            // Arrange
            var dto = new CreateCanteenDTO();

            // Act
            var result = await _controller.Create(dto);

            // Assert
            result.Result.Should().BeOfType<ForbidResult>();
            _mockCanteenService.Verify(
                x => x.CreateAsync(It.IsAny<CreateCanteenDTO>()),
                Times.Never
            );
        }

        [Fact]
        public async Task GetAll_ReturnsOkResultWithCanteens()
        {
            // Arrange
            var canteens = new List<Canteen>
            {
                new()
                {
                    Id = 1,
                    City = City.Breda,
                    Location = Location.LA,
                },
                new()
                {
                    Id = 2,
                    City = City.Tilburg,
                    Location = Location.TA,
                },
            };

            var expectedCanteens = canteens.Select(c => new CanteenDTO(c)).ToList();
            _mockCanteenService.Setup(x => x.GetAllAsync()).ReturnsAsync(expectedCanteens);

            // Act
            var result = await _controller.GetAll();

            // Assert
            var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
            var canteensResult = okResult
                .Value.Should()
                .BeAssignableTo<IEnumerable<CanteenDTO>>()
                .Subject;
            canteensResult.Should().BeEquivalentTo(expectedCanteens);
        }
    }
}

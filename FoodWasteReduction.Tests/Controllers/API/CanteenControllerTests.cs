using FluentAssertions;
using FoodWasteReduction.Api.Controllers;
using FoodWasteReduction.Api.Repositories.Interfaces;
using FoodWasteReduction.Core.DTOs;
using FoodWasteReduction.Core.Entities;
using FoodWasteReduction.Core.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace FoodWasteReduction.Tests.Controllers.Api
{
    public class CanteenControllerTests : ApiControllerTestBase
    {
        private readonly Mock<ICanteenRepository> _mockRepository;
        private readonly CanteensController _controller;

        public CanteenControllerTests()
        {
            _mockRepository = new Mock<ICanteenRepository>();
            _controller = new CanteensController(_mockRepository.Object);
            SetupController(_controller);
        }

        [Fact]
        public async Task Create_WithValidInput_ReturnsCreatedCanteen()
        {
            // Arrange
            SetupUserRole("CanteenStaff", _controller);
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = User },
            };

            var dto = new CreateCanteenDTO
            {
                City = City.Breda,
                Location = Location.LA,
                ServesWarmMeals = true,
            };

            var createdCanteen = new Canteen
            {
                Id = 1,
                City = dto.City,
                Location = dto.Location,
                ServesWarmMeals = dto.ServesWarmMeals,
            };

            _mockRepository
                .Setup(r => r.CreateAsync(It.IsAny<Canteen>()))
                .ReturnsAsync(createdCanteen);

            // Act
            var result = await _controller.Create(dto);

            // Assert
            var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
            var returnedCanteen = okResult.Value.Should().BeOfType<Canteen>().Subject;
            returnedCanteen.City.Should().Be(dto.City);
            returnedCanteen.Location.Should().Be(dto.Location);
            returnedCanteen.ServesWarmMeals.Should().Be(dto.ServesWarmMeals);
        }

        [Fact]
        public async Task Create_WithoutAuthorization_ReturnsUnauthorized()
        {
            // Arrange
            var dto = new CreateCanteenDTO();

            // Act
            var result = await _controller.Create(dto);

            // Assert
            result.Result.Should().BeOfType<ForbidResult>();
        }

        [Fact]
        public async Task GetAll_ReturnsAllCanteens()
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

            _mockRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(canteens);

            // Act
            var result = await _controller.GetAll();

            // Assert
            var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
            var returnedCanteens = okResult
                .Value.Should()
                .BeAssignableTo<IEnumerable<Canteen>>()
                .Subject;
            returnedCanteens.Should().HaveCount(2);
            returnedCanteens.Should().BeEquivalentTo(canteens);
        }

        [Fact]
        public async Task GetAll_WhenEmpty_ReturnsEmptyList()
        {
            // Arrange
            _mockRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Canteen>());

            // Act
            var result = await _controller.GetAll();

            // Assert
            var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
            var returnedCanteens = okResult
                .Value.Should()
                .BeAssignableTo<IEnumerable<Canteen>>()
                .Subject;
            returnedCanteens.Should().BeEmpty();
        }
    }
}

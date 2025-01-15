using FluentAssertions;
using FoodWasteReduction.Application.DTOs;
using FoodWasteReduction.Application.Services;
using FoodWasteReduction.Core.Entities;
using FoodWasteReduction.Core.Enums;
using FoodWasteReduction.Core.Interfaces.Repositories;
using Moq;

namespace FoodWasteReduction.Tests.Services.Application
{
    public class CanteenServiceTests
    {
        private readonly Mock<ICanteenRepository> _mockCanteenRepository;
        private readonly CanteenService _service;

        public CanteenServiceTests()
        {
            _mockCanteenRepository = new Mock<ICanteenRepository>();
            _service = new CanteenService(_mockCanteenRepository.Object);
        }

        [Fact]
        public async Task CreateAsync_WithValidData_ReturnsSuccess()
        {
            // Arrange
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

            _mockCanteenRepository
                .Setup(x => x.CreateAsync(It.IsAny<Canteen>()))
                .ReturnsAsync(createdCanteen);

            // Act
            var (success, canteen, error) = await _service.CreateAsync(dto);

            // Assert
            success.Should().BeTrue();
            error.Should().BeNull();
            canteen.Should().NotBeNull();
            canteen!.City.Should().Be(dto.City);
            canteen.Location.Should().Be(dto.Location);
        }

        [Fact]
        public async Task CreateAsync_WhenRepositoryFails_ReturnsError()
        {
            // Arrange
            var dto = new CreateCanteenDTO { City = City.Breda, Location = Location.LA };

            _mockCanteenRepository
                .Setup(x => x.CreateAsync(It.IsAny<Canteen>()))
                .ReturnsAsync((Canteen)null!);

            // Act
            var (success, canteen, error) = await _service.CreateAsync(dto);

            // Assert
            success.Should().BeFalse();
            canteen.Should().BeNull();
            error.Should().Be("Failed to create canteen");
        }

        [Fact]
        public async Task GetAllAsync_ReturnsAllCanteens()
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

            _mockCanteenRepository.Setup(x => x.GetAllAsync()).ReturnsAsync(canteens);

            // Act
            var result = await _service.GetAllAsync();

            // Assert
            result.Should().HaveCount(2);
            result.Should().BeEquivalentTo(canteens.Select(c => new CanteenDTO(c)));
        }
    }
}

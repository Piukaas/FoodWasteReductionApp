using FluentAssertions;
using FoodWasteReduction.Application.DTOs;
using FoodWasteReduction.Application.Services;
using FoodWasteReduction.Core.Entities;
using FoodWasteReduction.Core.Interfaces.Repositories;
using Moq;

namespace FoodWasteReduction.Tests.Services.Application
{
    public class ProductServiceTests
    {
        private readonly Mock<IProductRepository> _mockRepository;
        private readonly ProductService _service;

        public ProductServiceTests()
        {
            _mockRepository = new Mock<IProductRepository>();
            _service = new ProductService(_mockRepository.Object);
        }

        [Fact]
        public async Task CreateAsync_WithValidInput_ReturnsSuccess()
        {
            // Arrange
            var dto = new CreateProductDTO
            {
                Name = "Test Product",
                ContainsAlcohol = true,
                ImageUrl = "test.jpg",
            };

            var createdProduct = new Product
            {
                Id = 1,
                Name = dto.Name,
                ContainsAlcohol = dto.ContainsAlcohol,
                ImageUrl = dto.ImageUrl,
            };

            _mockRepository
                .Setup(r => r.CreateProductAsync(It.IsAny<Product>()))
                .ReturnsAsync(createdProduct);

            // Act
            var (success, product, error) = await _service.CreateAsync(dto);

            // Assert
            success.Should().BeTrue();
            product.Should().NotBeNull();
            error.Should().BeNull();
            product!.Name.Should().Be(dto.Name);
            product.ContainsAlcohol.Should().Be(dto.ContainsAlcohol);
        }

        [Fact]
        public async Task CreateAsync_WhenRepositoryFails_ReturnsError()
        {
            // Arrange
            var dto = new CreateProductDTO { Name = "Test Product" };

            _mockRepository
                .Setup(r => r.CreateProductAsync(It.IsAny<Product>()))
                .ReturnsAsync((Product)null!);

            // Act
            var (success, product, error) = await _service.CreateAsync(dto);

            // Assert
            success.Should().BeFalse();
            product.Should().BeNull();
            error.Should().Be("Failed to create product");
        }
    }
}

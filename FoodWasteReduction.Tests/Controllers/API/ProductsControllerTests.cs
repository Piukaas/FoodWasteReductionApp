using FluentAssertions;
using FoodWasteReduction.Api.Controllers;
using FoodWasteReduction.Api.Repositories.Interfaces;
using FoodWasteReduction.Application.DTOs;
using FoodWasteReduction.Core.Entities;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace FoodWasteReduction.Tests.Controllers.Api
{
    public class ProductsControllerTests : ApiControllerTestBase
    {
        private readonly Mock<IProductRepository> _mockRepository;
        private readonly ProductsController _controller;

        public ProductsControllerTests()
        {
            _mockRepository = new Mock<IProductRepository>();
            _controller = new ProductsController(_mockRepository.Object);
            SetupController(_controller);
        }

        [Fact]
        public async Task CreateProduct_WithoutAuthorization_ReturnsForbidden()
        {
            // Arrange
            var dto = new CreateProductDTO { Name = "Test Product" };

            // Act
            var result = await _controller.CreateProduct(dto);

            // Assert
            result.Result.Should().BeOfType<ForbidResult>();
        }

        [Fact]
        public async Task CreateProduct_WithInvalidModel_ReturnsBadRequest()
        {
            // Arrange
            SetupUserRole("CanteenStaff", _controller);
            _controller.ModelState.AddModelError("Name", "Required");
            var dto = new CreateProductDTO();

            // Act
            var result = await _controller.CreateProduct(dto);

            // Assert
            result.Result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task CreateProduct_WithValidInput_ReturnsCreatedProduct()
        {
            // Arrange
            SetupUserRole("CanteenStaff", _controller);
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
            var result = await _controller.CreateProduct(dto);

            // Assert
            var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
            var returnedProduct = okResult.Value.Should().BeOfType<Product>().Subject;
            returnedProduct.Should().BeEquivalentTo(createdProduct);
        }
    }
}

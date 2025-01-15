using FluentAssertions;
using FoodWasteReduction.Api.Controllers;
using FoodWasteReduction.Application.DTOs;
using FoodWasteReduction.Application.Services.Interfaces;
using FoodWasteReduction.Core.Entities;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace FoodWasteReduction.Tests.Controllers.Api
{
    public class ProductsControllerTests : ApiControllerTestBase
    {
        private readonly Mock<IProductService> _mockService;
        private readonly ProductsController _controller;

        public ProductsControllerTests()
        {
            _mockService = new Mock<IProductService>();
            _controller = new ProductsController(_mockService.Object);
            SetupController(_controller);
        }

        [Fact]
        public async Task CreateProduct_WithoutAuthorization_ReturnsForbidden()
        {
            // Arrange
            var dto = new CreateProductDTO();

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
            var dto = new CreateProductDTO { Name = "Test Product" };
            var productDto = new ProductDTO(new Product { Id = 1, Name = dto.Name });

            _mockService.Setup(s => s.CreateAsync(dto)).ReturnsAsync((true, productDto, null));

            // Act
            var result = await _controller.CreateProduct(dto);

            // Assert
            var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
            okResult.Value.Should().Be(productDto);
        }

        [Fact]
        public async Task CreateProduct_WithServiceError_ReturnsBadRequest()
        {
            // Arrange
            SetupUserRole("CanteenStaff", _controller);
            var dto = new CreateProductDTO();
            _mockService
                .Setup(s => s.CreateAsync(dto))
                .ReturnsAsync((false, null, "Error message"));

            // Act
            var result = await _controller.CreateProduct(dto);

            // Assert
            var badRequest = result.Result.Should().BeOfType<BadRequestObjectResult>().Subject;
            badRequest.Value.Should().Be("Error message");
        }
    }
}

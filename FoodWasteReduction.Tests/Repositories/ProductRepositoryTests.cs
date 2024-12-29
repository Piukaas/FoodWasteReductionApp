using FluentAssertions;
using FoodWasteReduction.Api.Repositories;
using FoodWasteReduction.Core.Entities;

namespace FoodWasteReduction.Tests.Repositories
{
    public class ProductRepositoryTests : RepositoryTestBase
    {
        private readonly ProductRepository _repository;

        public ProductRepositoryTests()
            : base()
        {
            _repository = new ProductRepository(Context);
        }

        private async Task<List<Product>> CreateTestProducts()
        {
            var products = new List<Product>
            {
                new() { Name = "Product 1", ContainsAlcohol = false },
                new() { Name = "Product 2", ContainsAlcohol = true },
            };

            foreach (var product in products)
            {
                await _repository.CreateProductAsync(product);
            }

            return products;
        }

        [Fact]
        public async Task CreateProductAsync_ValidProduct_CreatesAndReturnsProduct()
        {
            // Arrange
            var product = new Product { Name = "Test Product", ContainsAlcohol = false };

            // Act
            var result = await _repository.CreateProductAsync(product);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().BeGreaterThan(0);
            result.Name.Should().Be(product.Name);
        }

        [Fact]
        public async Task GetProductsByIdsAsync_ExistingIds_ReturnsProducts()
        {
            // Arrange
            var products = await CreateTestProducts();
            var ids = products.Select(p => p.Id).ToList();

            // Act
            var result = await _repository.GetProductsByIdsAsync(ids);

            // Assert
            result.Should().HaveCount(products.Count);
            result.Select(p => p.Id).Should().BeEquivalentTo(ids);
        }

        [Fact]
        public async Task GetAllProductsAsync_ReturnsAllProducts()
        {
            // Arrange
            var products = await CreateTestProducts();

            // Act
            var result = await _repository.GetAllProductsAsync();

            // Assert
            result.Should().HaveCount(products.Count);
        }
    }
}

using FoodWasteReduction.Application.DTOs;
using FoodWasteReduction.Application.Services.Interfaces;
using FoodWasteReduction.Core.Entities;
using FoodWasteReduction.Core.Interfaces.Repositories;

namespace FoodWasteReduction.Application.Services
{
    public class ProductService(IProductRepository productRepository) : IProductService
    {
        private readonly IProductRepository _productRepository = productRepository;

        public async Task<(bool success, ProductDTO? product, string? error)> CreateAsync(
            CreateProductDTO dto
        )
        {
            var product = new Product
            {
                Name = dto.Name,
                ContainsAlcohol = dto.ContainsAlcohol,
                ImageUrl = dto.ImageUrl,
            };

            var result = await _productRepository.CreateProductAsync(product);
            if (result == null)
                return (false, null, "Failed to create product");

            return (true, new ProductDTO(result), null);
        }
    }
}

using FoodWasteReduction.Application.DTOs;

namespace FoodWasteReduction.Application.Services.Interfaces
{
    public interface IProductService
    {
        Task<(bool success, ProductDTO? product, string? error)> CreateAsync(CreateProductDTO dto);
    }
}

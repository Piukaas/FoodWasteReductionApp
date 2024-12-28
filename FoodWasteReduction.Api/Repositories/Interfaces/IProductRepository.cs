using FoodWasteReduction.Core.Entities;

namespace FoodWasteReduction.Api.Repositories.Interfaces
{
    public interface IProductRepository
    {
        Task<List<Product>> GetProductsByIdsAsync(List<int> productIds);
        Task<Product> CreateProductAsync(Product product);
    }
}

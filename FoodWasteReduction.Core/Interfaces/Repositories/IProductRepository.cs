using FoodWasteReduction.Core.Entities;

namespace FoodWasteReduction.Core.Interfaces.Repositories
{
    public interface IProductRepository
    {
        Task<List<Product>> GetProductsByIdsAsync(List<int> productIds);
        Task<Product> CreateProductAsync(Product product);
    }
}

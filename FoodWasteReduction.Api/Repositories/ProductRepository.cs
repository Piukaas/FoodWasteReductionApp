using FoodWasteReduction.Api.Repositories.Interfaces;
using FoodWasteReduction.Core.Entities;
using FoodWasteReduction.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FoodWasteReduction.Api.Repositories
{
    public class ProductRepository(ApplicationDbContext context) : IProductRepository
    {
        private readonly ApplicationDbContext _context = context;

        public async Task<List<Product>> GetProductsByIdsAsync(List<int> productIds)
        {
            return await _context.Products!.Where(p => productIds.Contains(p.Id)).ToListAsync();
        }

        public async Task<Product> CreateProductAsync(Product product)
        {
            await _context.Products!.AddAsync(product);
            await _context.SaveChangesAsync();
            return product;
        }

        public IQueryable<Product> GetProductsGraphQL()
        {
            return _context.Products!;
        }
    }
}

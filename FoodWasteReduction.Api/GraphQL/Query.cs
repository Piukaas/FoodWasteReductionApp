using FoodWasteReduction.Api.Repositories.Interfaces;
using FoodWasteReduction.Core.Entities;
using HotChocolate.Authorization;

namespace FoodWasteReduction.Api.GraphQL
{
    [Authorize]
    public class Query(IPackageRepository packageRepository, IProductRepository productRepository)
    {
        private readonly IPackageRepository _packageRepository = packageRepository;
        private readonly IProductRepository _productRepository = productRepository;

        [UseProjection]
        [HotChocolate.Data.UseFiltering]
        [HotChocolate.Data.UseSorting]
        [Authorize]
        public async Task<IQueryable<Package>> GetPackages()
        {
            var packages = await _packageRepository.GetPackagesAsync();
            return packages.AsQueryable();
        }

        [UseProjection]
        [HotChocolate.Data.UseFiltering]
        [HotChocolate.Data.UseSorting]
        [Authorize]
        public async Task<IQueryable<Product>> GetProducts()
        {
            var products = await _productRepository.GetAllProductsAsync();
            return products.AsQueryable();
        }
    }
}

using FoodWasteReduction.Application.DTOs;
using FoodWasteReduction.Application.Services.Interfaces;
using FoodWasteReduction.Core.Entities;
using FoodWasteReduction.Core.Interfaces.Repositories;

namespace FoodWasteReduction.Application.Services
{
    public class PackageService(
        IPackageRepository packageRepository,
        ICanteenRepository canteenRepository,
        IProductRepository productRepository
    ) : IPackageService
    {
        private readonly IPackageRepository _packageRepository = packageRepository;
        private readonly ICanteenRepository _canteenRepository = canteenRepository;
        private readonly IProductRepository _productRepository = productRepository;

        public async Task<(bool success, PackageDTO? package, string? error)> CreateAsync(
            CreatePackageDTO dto
        )
        {
            var canteen = await _canteenRepository.GetByIdAsync(dto.CanteenId);
            if (canteen == null)
                return (false, null, "Invalid canteen ID");

            var products = await _productRepository.GetProductsByIdsAsync(dto.ProductIds);
            if (products.Count != dto.ProductIds.Count)
                return (false, null, "One or more product IDs are invalid");

            var package = new Package
            {
                Name = dto.Name,
                City = dto.City,
                CanteenId = dto.CanteenId,
                Type = dto.Type,
                PickupTime = dto.PickupTime,
                ExpiryTime = dto.PickupTime.AddHours(2),
                Price = dto.Price,
                Is18Plus = products.Any(p => p.ContainsAlcohol),
                Products = products,
            };

            var result = await _packageRepository.CreatePackageAsync(package);
            return (true, new PackageDTO(result), null);
        }

        public async Task<(bool success, string? error)> DeleteAsync(int id)
        {
            var package = await _packageRepository.GetByIdAsync(id);
            if (package == null)
                return (false, "Package not found");

            if (package.ReservedById != null)
                return (false, "Cannot delete package that is already reserved");

            await _packageRepository.DeletePackageAsync(package);
            return (true, null);
        }

        public async Task<(bool success, PackageDTO? package, string? error)> UpdateAsync(
            int id,
            CreatePackageDTO dto
        )
        {
            var package = await _packageRepository.GetPackageWithProductsAsync(id);
            if (package == null)
                return (false, null, "Package not found");

            if (package.ReservedById != null)
                return (false, null, "Cannot update package that is already reserved");

            var products = await _productRepository.GetProductsByIdsAsync(dto.ProductIds);
            if (products.Count != dto.ProductIds.Count)
                return (false, null, "One or more product IDs are invalid");

            package.Name = dto.Name;
            package.Type = dto.Type;
            package.PickupTime = dto.PickupTime;
            package.ExpiryTime = dto.PickupTime.AddHours(2);
            package.Price = dto.Price;
            package.Is18Plus = products.Any(p => p.ContainsAlcohol);
            package.Products = products;

            var result = await _packageRepository.UpdatePackageAsync(package);
            return (true, new PackageDTO(result), null);
        }
    }
}

using FoodWasteReduction.Core.Entities;
using FoodWasteReduction.Core.Enums;
using FoodWasteReduction.Web.Models;

namespace FoodWasteReduction.Web.Services.Interfaces
{
    public interface IPackageService
    {
        Task<Package> CreatePackage(PackageViewModel model);
        Task<Product> CreateProduct(ProductViewModel model);
        Task<Package> UpdatePackage(int id, PackageViewModel model);
        Task DeletePackage(int id);
        Task<IEnumerable<Package>> GetAvailablePackages(City? city = null, MealType? type = null);
        Task<IEnumerable<Package>> GetReservedPackages(string? userId = null);
        Task<IEnumerable<Package>> GetPackagesForManagement(int? canteenId = null);
        Task<IEnumerable<Product>> GetProducts();
    }
}

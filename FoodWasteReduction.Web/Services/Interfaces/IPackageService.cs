using FoodWasteReduction.Application.DTOs;
using FoodWasteReduction.Application.DTOs.Json;
using FoodWasteReduction.Core.Enums;
using FoodWasteReduction.Web.Models;

namespace FoodWasteReduction.Web.Services.Interfaces
{
    public interface IPackageService
    {
        Task<JsonPackageDTO> CreatePackage(PackageViewModel model);
        Task<JsonProductDTO> CreateProduct(ProductViewModel model);
        Task<JsonPackageDTO> UpdatePackage(int id, PackageViewModel model);
        Task DeletePackage(int id);
        Task<JsonPackageDTO?> GetPackage(int id);
        Task<IEnumerable<JsonPackageDTO>> GetAvailablePackages(
            City? city = null,
            MealType? type = null
        );
        Task<IEnumerable<JsonPackageDTO>> GetReservedPackages(string? userId = null);
        Task<IEnumerable<JsonPackageDTO>> GetPackagesForManagement(int? canteenId = null);
        Task<IEnumerable<JsonProductDTO>> GetProducts();
    }
}

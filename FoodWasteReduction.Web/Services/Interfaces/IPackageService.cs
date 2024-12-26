using FoodWasteReduction.Core.Entities;
using FoodWasteReduction.Core.Enums;

namespace FoodWasteReduction.Web.Services.Interfaces
{
    public interface IPackageService
    {
        Task<IEnumerable<Package>> GetAvailablePackages(City? city = null, MealType? type = null);
        Task<IEnumerable<Package>> GetReservedPackages(string? userId = null);
        Task<IEnumerable<Product>> GetProducts();
    }
}

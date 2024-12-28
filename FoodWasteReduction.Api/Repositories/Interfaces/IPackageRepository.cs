using FoodWasteReduction.Core.Entities;

namespace FoodWasteReduction.Api.Repositories.Interfaces
{
    public interface IPackageRepository
    {
        Task<Package> CreatePackageAsync(Package package);
        Task<Package?> GetByIdAsync(int id);
        Task<Package?> GetPackageWithProductsAsync(int id);
        Task DeletePackageAsync(Package package);
        Task<Package> UpdatePackageAsync(Package package);
        Task<bool> HasReservationOnDateAsync(string userId, DateTime date);
        Task<Package?> GetPackageWithDetailsAsync(int packageId);
        Task<Package> ReservePackageAsync(Package package, string userId);
    }
}

using FoodWasteReduction.Core.Entities;

namespace FoodWasteReduction.Api.Repositories.Interfaces
{
    public interface ICanteenRepository
    {
        Task<Canteen?> GetByIdAsync(int id);
        Task<Canteen> CreateAsync(Canteen canteen);
        Task<IEnumerable<Canteen>> GetAllAsync();
    }
}

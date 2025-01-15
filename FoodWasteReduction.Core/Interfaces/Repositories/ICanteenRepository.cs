using FoodWasteReduction.Core.Entities;

namespace FoodWasteReduction.Core.Interfaces.Repositories
{
    public interface ICanteenRepository
    {
        Task<Canteen?> GetByIdAsync(int id);
        Task<Canteen> CreateAsync(Canteen canteen);
        Task<IEnumerable<Canteen>> GetAllAsync();
    }
}

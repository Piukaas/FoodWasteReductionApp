using FoodWasteReduction.Core.Entities;

namespace FoodWasteReduction.Core.Interfaces.Repositories
{
    public interface ICanteenStaffRepository
    {
        Task<CanteenStaff?> GetByIdAsync(string id);
        Task<CanteenStaff> CreateAsync(CanteenStaff canteenStaff);
        Task<CanteenStaff?> GetCanteenStaffWithDetailsAsync(string userId);
    }
}

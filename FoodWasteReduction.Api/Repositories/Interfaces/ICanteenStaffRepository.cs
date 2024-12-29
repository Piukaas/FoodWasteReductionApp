using FoodWasteReduction.Core.Entities;

namespace FoodWasteReduction.Api.Repositories.Interfaces
{
    public interface ICanteenStaffRepository
    {
        Task<CanteenStaff?> GetByIdAsync(string id);
        Task<CanteenStaff> CreateAsync(CanteenStaff canteenStaff);
        Task<CanteenStaff?> GetCanteenStaffWithDetailsAsync(string userId);
    }
}

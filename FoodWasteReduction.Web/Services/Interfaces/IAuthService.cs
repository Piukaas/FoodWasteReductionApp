using FoodWasteReduction.Web.Models.Auth;

namespace FoodWasteReduction.Web.Services.Interfaces
{
    public interface IAuthService
    {
        Task<(bool success, string token, object? userData)> Login(LoginViewModel model);
        Task<bool> RegisterStudent(RegisterStudentViewModel model);
        Task<bool> RegisterCanteenStaff(RegisterCanteenStaffViewModel model);
    }
}

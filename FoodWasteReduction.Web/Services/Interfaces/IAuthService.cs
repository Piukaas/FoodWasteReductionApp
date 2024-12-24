using FoodWasteReduction.Web.Models.Auth;

public interface IAuthService
{
    Task<(bool success, string token)> Login(LoginViewModel model);
    Task<bool> RegisterStudent(RegisterStudentViewModel model);
    Task<bool> RegisterCanteenStaff(RegisterCanteenStaffViewModel model);
}

using FoodWasteReduction.Web.Models.Auth;

public interface IAuthService
{
    Task<bool> RegisterStudent(RegisterStudentViewModel model);
    Task<bool> RegisterCanteenStaff(RegisterCanteenStaffViewModel model);
    Task<bool> Login(LoginViewModel model);
}

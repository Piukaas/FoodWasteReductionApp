using FoodWasteReduction.Application.DTOs.Auth;

namespace FoodWasteReduction.Application.Services.Interfaces
{
    public interface IAuthService
    {
        Task<(bool success, string? error)> RegisterStudentAsync(RegisterStudentDTO model);
        Task<(bool success, string? error)> RegisterCanteenStaffAsync(
            RegisterCanteenStaffDTO model
        );
        Task<(bool success, object? response, string? error)> LoginAsync(LoginDTO model);
    }
}

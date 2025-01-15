using FoodWasteReduction.Application.DTOs;

namespace FoodWasteReduction.Application.Services.Interfaces
{
    public interface IPackageService
    {
        Task<(bool success, PackageDTO? package, string? error)> CreateAsync(CreatePackageDTO dto);
        Task<(bool success, string? error)> DeleteAsync(int id);
        Task<(bool success, PackageDTO? package, string? error)> UpdateAsync(
            int id,
            CreatePackageDTO dto
        );
    }
}

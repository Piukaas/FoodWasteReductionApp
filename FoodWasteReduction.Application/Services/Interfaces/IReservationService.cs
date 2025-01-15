using FoodWasteReduction.Application.DTOs;

namespace FoodWasteReduction.Application.Services.Interfaces
{
    public interface IReservationService
    {
        Task<(bool success, PackageDTO? package, ErrorResponse? error)> ReservePackageAsync(
            ReservePackageDTO dto
        );
    }
}

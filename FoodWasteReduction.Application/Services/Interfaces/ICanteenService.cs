using FoodWasteReduction.Application.DTOs;

namespace FoodWasteReduction.Application.Services.Interfaces
{
    public interface ICanteenService
    {
        Task<(bool success, CanteenDTO? canteen, string? error)> CreateAsync(CreateCanteenDTO dto);
        Task<IEnumerable<CanteenDTO>> GetAllAsync();
    }
}

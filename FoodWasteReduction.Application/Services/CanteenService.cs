using FoodWasteReduction.Application.DTOs;
using FoodWasteReduction.Application.Services.Interfaces;
using FoodWasteReduction.Core.Entities;
using FoodWasteReduction.Core.Interfaces.Repositories;

namespace FoodWasteReduction.Application.Services
{
    public class CanteenService(ICanteenRepository canteenRepository) : ICanteenService
    {
        private readonly ICanteenRepository _canteenRepository = canteenRepository;

        public async Task<(bool success, CanteenDTO? canteen, string? error)> CreateAsync(
            CreateCanteenDTO dto
        )
        {
            var canteen = new Canteen
            {
                City = dto.City,
                Location = dto.Location,
                ServesWarmMeals = dto.ServesWarmMeals,
            };

            var result = await _canteenRepository.CreateAsync(canteen);
            if (result == null)
                return (false, null, "Failed to create canteen");

            return (true, new CanteenDTO(result), null);
        }

        public async Task<IEnumerable<CanteenDTO>> GetAllAsync()
        {
            var canteens = await _canteenRepository.GetAllAsync();
            return canteens.Select(c => new CanteenDTO(c));
        }
    }
}

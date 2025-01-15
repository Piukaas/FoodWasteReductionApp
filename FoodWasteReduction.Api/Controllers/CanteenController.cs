using FoodWasteReduction.Api.Repositories.Interfaces;
using FoodWasteReduction.Application.DTOs;
using FoodWasteReduction.Core.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FoodWasteReduction.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CanteensController(ICanteenRepository canteenRepository) : ControllerBase
    {
        private readonly ICanteenRepository _canteenRepository = canteenRepository;

        [Authorize(Roles = "CanteenStaff")]
        [HttpPost]
        public async Task<ActionResult<Canteen>> Create(CreateCanteenDTO dto)
        {
            if (!User.IsInRole("CanteenStaff"))
                return Forbid();

            var canteen = new Canteen
            {
                City = dto.City,
                Location = dto.Location,
                ServesWarmMeals = dto.ServesWarmMeals,
            };

            canteen = await _canteenRepository.CreateAsync(canteen);
            return Ok(canteen);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Canteen>>> GetAll()
        {
            var canteens = await _canteenRepository.GetAllAsync();
            return Ok(canteens);
        }
    }
}

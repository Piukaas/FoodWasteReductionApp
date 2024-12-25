using FoodWasteReduction.Core.DTOs;
using FoodWasteReduction.Core.Entities;
using FoodWasteReduction.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FoodWasteReduction.Api.Controllers
{
    [Authorize(Roles = "CanteenStaff")]
    [ApiController]
    [Route("api/[controller]")]
    public class CanteensController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CanteensController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<ActionResult<Canteen>> Create(CreateCanteenDTO dto)
        {
            var canteen = new Canteen
            {
                City = dto.City,
                Location = dto.Location,
                ServesWarmMeals = dto.ServesWarmMeals,
            };

            _context.Canteens?.Add(canteen);
            await _context.SaveChangesAsync();

            return Ok(canteen);
        }
    }
}

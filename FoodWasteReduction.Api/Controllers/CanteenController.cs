using FoodWasteReduction.Core.DTOs;
using FoodWasteReduction.Core.Entities;
using FoodWasteReduction.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FoodWasteReduction.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CanteensController(ApplicationDbContext context) : ControllerBase
    {
        private readonly ApplicationDbContext _context = context;

        [Authorize(Roles = "CanteenStaff")]
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

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Canteen>>> GetAll()
        {
            if (_context.Canteens == null)
                return NotFound();

            var canteens = await _context.Canteens?.ToListAsync()!;
            return Ok(canteens);
        }
    }
}

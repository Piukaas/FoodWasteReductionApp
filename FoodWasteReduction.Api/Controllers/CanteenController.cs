using FoodWasteReduction.Application.DTOs;
using FoodWasteReduction.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FoodWasteReduction.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CanteensController(ICanteenService canteenService) : ControllerBase
    {
        private readonly ICanteenService _canteenService = canteenService;

        [Authorize(Roles = "CanteenStaff")]
        [HttpPost]
        public async Task<ActionResult<CanteenDTO>> Create(CreateCanteenDTO dto)
        {
            if (!User.IsInRole("CanteenStaff"))
                return Forbid();

            var (success, canteen, error) = await _canteenService.CreateAsync(dto);
            if (!success)
                return BadRequest(error);

            return Ok(canteen);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CanteenDTO>>> GetAll()
        {
            var canteens = await _canteenService.GetAllAsync();
            return Ok(canteens);
        }
    }
}

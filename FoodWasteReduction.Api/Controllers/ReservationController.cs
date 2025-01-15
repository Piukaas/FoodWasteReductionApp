using FoodWasteReduction.Application.DTOs;
using FoodWasteReduction.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FoodWasteReduction.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Student")]
    public class ReservationController(IReservationService reservationService) : ControllerBase
    {
        private readonly IReservationService _reservationService = reservationService;

        [HttpPost]
        public async Task<IActionResult> ReservePackage([FromBody] ReservePackageDTO dto)
        {
            if (!User.IsInRole("Student"))
                return Forbid();

            var (success, package, error) = await _reservationService.ReservePackageAsync(dto);

            if (!success)
            {
                return error!.Code switch
                {
                    "NOT_FOUND" => NotFound(error.Message),
                    _ => BadRequest(error),
                };
            }

            return Ok(package);
        }
    }
}

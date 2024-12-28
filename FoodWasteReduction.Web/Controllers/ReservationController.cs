using System.Text.Json.Nodes;
using FoodWasteReduction.Web.Models;
using FoodWasteReduction.Web.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FoodWasteReduction.Web.Controllers
{
    public class ReservationController(IReservationService reservationService) : Controller
    {
        private readonly IReservationService _reservationService = reservationService;

        [HttpPost]
        public async Task<IActionResult> ReservePackage([FromBody] ReservePackageRequest request)
        {
            if (request == null)
                return BadRequest("Invalid request data");

            Console.WriteLine($"Reserving package with id: {request.PackageId}");
            var userData = HttpContext.Session.GetString("UserData");

            if (string.IsNullOrEmpty(userData))
                return Unauthorized();

            var jsonObject = JsonNode.Parse(userData);
            var userId = jsonObject?["Id"]?.GetValue<string>();

            if (string.IsNullOrEmpty(userId))
                return Unauthorized("User ID not found in session");

            (bool success, string? errorMessage) = await _reservationService.ReservePackage(
                request.PackageId,
                userId
            );

            if (!success)
                return BadRequest(new { message = errorMessage });

            return Ok();
        }
    }
}

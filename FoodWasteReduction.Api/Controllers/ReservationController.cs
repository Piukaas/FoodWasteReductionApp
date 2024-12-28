using FoodWasteReduction.Core.DTOs;
using FoodWasteReduction.Core.Entities;
using FoodWasteReduction.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FoodWasteReduction.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Student")]
    public class ReservationController(
        ApplicationDbContext context,
        UserManager<ApplicationUser> userManager
    ) : ControllerBase
    {
        private readonly ApplicationDbContext _context = context;
        private readonly UserManager<ApplicationUser> _userManager = userManager;

        [HttpPost]
        public async Task<IActionResult> ReservePackage([FromBody] ReservePackageDTO dto)
        {
            if (!User.IsInRole("Student"))
                return Forbid();

            var package = await _context
                .Packages?.Include(p => p.Products)
                .FirstOrDefaultAsync(p => p.Id == dto.PackageId)!;

            if (package == null)
                return NotFound("Package not found");

            if (package.ReservedById != null)
                return BadRequest(
                    new ErrorResponse
                    {
                        Code = "ALREADY_RESERVED",
                        Message = "Dit pakket is al gereserveerd",
                    }
                );

            var user = await _userManager.FindByIdAsync(dto.UserId);
            if (user == null)
                return NotFound("User not found");

            // Check age for 18+ packages
            if (user is Student student && package.Is18Plus)
            {
                var ageAtPickup = package.PickupTime.Year - student.DateOfBirth.Year;
                if (package.PickupTime.Date < student.DateOfBirth.Date.AddYears(ageAtPickup))
                    ageAtPickup--;

                if (ageAtPickup < 18)
                    return BadRequest(
                        new ErrorResponse
                        {
                            Code = "AGE_RESTRICTION",
                            Message = "Je moet 18 jaar of ouder zijn op de ophaaldatum",
                        }
                    );
            }

            // Check for existing reservations on same date
            var existingReservation = await _context.Packages.AnyAsync(p =>
                p.ReservedById == dto.UserId && p.PickupTime.Date == package.PickupTime.Date
            );

            if (existingReservation)
                return BadRequest(
                    new ErrorResponse
                    {
                        Code = "DUPLICATE_RESERVATION",
                        Message = "Je hebt al een reservering op deze datum",
                    }
                );

            package.ReservedById = dto.UserId;
            await _context.SaveChangesAsync();

            return Ok(package);
        }
    }
}

using FoodWasteReduction.Application.DTOs;
using FoodWasteReduction.Core.Entities;
using FoodWasteReduction.Core.Interfaces.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace FoodWasteReduction.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Student")]
    public class ReservationController(
        IPackageRepository packageRepository,
        IStudentRepository studentRepository,
        UserManager<ApplicationUser> userManager
    ) : ControllerBase
    {
        private readonly IPackageRepository _packageRepository = packageRepository;
        private readonly IStudentRepository _studentRepository = studentRepository;
        private readonly UserManager<ApplicationUser> _userManager = userManager;

        [HttpPost]
        public async Task<IActionResult> ReservePackage([FromBody] ReservePackageDTO dto)
        {
            if (!User.IsInRole("Student"))
                return Forbid();

            var package = await _packageRepository.GetPackageWithDetailsAsync(dto.PackageId);

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

            var student = await _studentRepository.GetByIdAsync(dto.UserId);
            if (student == null)
                return NotFound("Student not found");

            // Check age for 18+ packages
            if (package.Is18Plus)
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
            var hasExistingReservation = await _packageRepository.HasReservationOnDateAsync(
                dto.UserId,
                package.PickupTime
            );

            if (hasExistingReservation)
                return BadRequest(
                    new ErrorResponse
                    {
                        Code = "DUPLICATE_RESERVATION",
                        Message = "Je hebt al een reservering op deze datum",
                    }
                );

            package = await _packageRepository.ReservePackageAsync(package, dto.UserId);
            return Ok(package);
        }
    }
}

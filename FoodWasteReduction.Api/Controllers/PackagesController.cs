using FoodWasteReduction.Application.DTOs;
using FoodWasteReduction.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FoodWasteReduction.Api.Controllers
{
    [Authorize(Roles = "CanteenStaff")]
    [ApiController]
    [Route("api/[controller]")]
    public class PackagesController(IPackageService packageService) : ControllerBase
    {
        private readonly IPackageService _packageService = packageService;

        [HttpPost]
        public async Task<ActionResult<PackageDTO>> Create(CreatePackageDTO dto)
        {
            if (!User.IsInRole("CanteenStaff"))
                return Forbid();

            var (success, package, error) = await _packageService.CreateAsync(dto);
            if (!success)
                return BadRequest(error);

            return Ok(package);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            if (!User.IsInRole("CanteenStaff"))
                return Forbid();

            var (success, error) = await _packageService.DeleteAsync(id);
            if (!success)
                return BadRequest(error);

            return NoContent();
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<PackageDTO>> Update(int id, CreatePackageDTO dto)
        {
            if (!User.IsInRole("CanteenStaff"))
                return Forbid();

            var (success, package, error) = await _packageService.UpdateAsync(id, dto);
            if (!success)
                return BadRequest(error);

            return Ok(package);
        }
    }
}

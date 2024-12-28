using FoodWasteReduction.Api.Repositories.Interfaces;
using FoodWasteReduction.Core.DTOs;
using FoodWasteReduction.Core.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FoodWasteReduction.Api.Controllers
{
    [Authorize(Roles = "CanteenStaff")]
    [ApiController]
    [Route("api/[controller]")]
    public class PackagesController(
        IPackageRepository packageRepository,
        ICanteenRepository canteenRepository,
        IProductRepository productRepository
    ) : ControllerBase
    {
        private readonly IPackageRepository _packageRepository = packageRepository;
        private readonly ICanteenRepository _canteenRepository = canteenRepository;
        private readonly IProductRepository _productRepository = productRepository;

        [HttpPost]
        public async Task<ActionResult<Package>> Create(CreatePackageDTO dto)
        {
            if (!User.IsInRole("CanteenStaff"))
                return Forbid();

            var canteen = await _canteenRepository.GetByIdAsync(dto.CanteenId);
            if (canteen == null)
                return BadRequest("Invalid canteen ID");

            var products = await _productRepository.GetProductsByIdsAsync(dto.ProductIds);
            if (products.Count != dto.ProductIds.Count)
                return BadRequest("One or more product IDs are invalid");

            var package = new Package
            {
                Name = dto.Name,
                City = dto.City,
                CanteenId = dto.CanteenId,
                Type = dto.Type,
                PickupTime = dto.PickupTime,
                ExpiryTime = dto.ExpiryTime,
                Price = dto.Price,
                Is18Plus = products.Any(p => p.ContainsAlcohol),
                Products = products,
            };

            package = await _packageRepository.CreatePackageAsync(package);
            return Ok(package);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            if (!User.IsInRole("CanteenStaff"))
                return Forbid();

            var package = await _packageRepository.GetByIdAsync(id);
            if (package == null)
                return NotFound();

            if (package.ReservedById != null)
                return BadRequest("Cannot delete package that is already reserved");

            await _packageRepository.DeletePackageAsync(package);
            return NoContent();
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Package>> Update(int id, CreatePackageDTO dto)
        {
            if (!User.IsInRole("CanteenStaff"))
                return Forbid();

            var package = await _packageRepository.GetByIdAsync(id);
            if (package == null)
                return NotFound();

            if (package.ReservedById != null)
                return BadRequest("Cannot update package that is already reserved");

            var canteen = await _canteenRepository.GetByIdAsync(dto.CanteenId);
            if (canteen == null)
                return BadRequest("Invalid canteen ID");

            var products = await _productRepository.GetProductsByIdsAsync(dto.ProductIds);
            if (products.Count != dto.ProductIds.Count)
                return BadRequest("One or more product IDs are invalid");

            package.Name = dto.Name;
            package.Type = dto.Type;
            package.PickupTime = dto.PickupTime;
            package.ExpiryTime = dto.ExpiryTime;
            package.Price = dto.Price;
            package.Is18Plus = products.Any(p => p.ContainsAlcohol);
            package.Products = products;

            package = await _packageRepository.UpdatePackageAsync(package);
            return Ok(package);
        }
    }
}

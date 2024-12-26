using FoodWasteReduction.Core.DTOs;
using FoodWasteReduction.Core.Entities;
using FoodWasteReduction.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FoodWasteReduction.Api.Controllers
{
    [Authorize(Roles = "CanteenStaff")]
    [ApiController]
    [Route("api/[controller]")]
    public class PackagesController(ApplicationDbContext context) : ControllerBase
    {
        private readonly ApplicationDbContext _context = context;

        [HttpPost]
        public async Task<ActionResult<Package>> Create(CreatePackageDTO dto)
        {
            if (!User.IsInRole("CanteenStaff"))
            {
                return Forbid();
            }

            if (_context.Canteens == null)
                return BadRequest("Canteens context is null");

            var canteen = await _context.Canteens.FindAsync(dto.CanteenId);
            if (canteen == null)
                return BadRequest("Invalid canteen ID");

            if (_context.Products == null)
                return BadRequest("Products context is null");

            var products = await _context
                .Set<Product>()
                .Where(p => dto.ProductIds.Contains(p.Id))
                .ToListAsync();

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

            _context.Packages?.Add(package);
            await _context.SaveChangesAsync();

            return Ok(package);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            if (!User.IsInRole("CanteenStaff"))
            {
                return Forbid();
            }

            var package = await _context
                .Packages?.Include(p => p.Products)
                .FirstOrDefaultAsync(p => p.Id == id)!;

            if (package == null)
                return NotFound();

            if (package.ReservedById != null)
                return BadRequest("Cannot delete package that is already reserved");

            _context.Packages.Remove(package);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Package>> Update(int id, CreatePackageDTO dto)
        {
            if (!User.IsInRole("CanteenStaff"))
            {
                return Forbid();
            }

            var package = await _context
                .Packages?.Include(p => p.Products)
                .FirstOrDefaultAsync(p => p.Id == id)!;

            if (package == null)
                return NotFound();

            if (package.ReservedById != null)
                return BadRequest("Cannot update package that is already reserved");

            if (_context.Canteens == null)
                return BadRequest("Canteens context is null");

            var canteen = await _context.Canteens.FindAsync(dto.CanteenId);
            if (canteen == null)
                return BadRequest("Invalid canteen ID");

            var products = await _context
                .Products?.Where(p => dto.ProductIds.Contains(p.Id))
                .ToListAsync()!;

            if (products.Count != dto.ProductIds.Count)
                return BadRequest("One or more product IDs are invalid");

            package.Name = dto.Name;
            package.City = dto.City;
            package.CanteenId = dto.CanteenId;
            package.Type = dto.Type;
            package.PickupTime = dto.PickupTime;
            package.ExpiryTime = dto.ExpiryTime;
            package.Price = dto.Price;
            package.Is18Plus = products.Any(p => p.ContainsAlcohol);
            package.Products = products;

            await _context.SaveChangesAsync();
            return Ok(package);
        }
    }
}

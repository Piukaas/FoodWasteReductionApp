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
    public class PackagesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public PackagesController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<ActionResult<Package>> Create(CreatePackageDTO dto)
        {
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
                Is18Plus = dto.Is18Plus,
                Products = products,
            };

            _context.Packages?.Add(package);
            await _context.SaveChangesAsync();

            return Ok(package);
        }
    }
}

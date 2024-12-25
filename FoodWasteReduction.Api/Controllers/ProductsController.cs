using FoodWasteReduction.Core.DTOs;
using FoodWasteReduction.Core.Entities;
using FoodWasteReduction.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FoodWasteReduction.Api.Controllers
{
    [Authorize(Roles = "CanteenStaff")]
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ProductsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<ActionResult<Product>> CreateProduct(CreateProductDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var product = new Product
            {
                Name = dto.Name,
                ContainsAlcohol = dto.ContainsAlcohol,
                ImageUrl = dto.ImageUrl,
            };

            _context.Products?.Add(product);
            await _context.SaveChangesAsync();

            return Ok(product);
        }
    }
}

using FoodWasteReduction.Application.DTOs;
using FoodWasteReduction.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FoodWasteReduction.Api.Controllers
{
    [Authorize(Roles = "CanteenStaff")]
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController(IProductService productService) : ControllerBase
    {
        private readonly IProductService _productService = productService;

        [HttpPost]
        public async Task<ActionResult<ProductDTO>> CreateProduct(CreateProductDTO dto)
        {
            if (!User.IsInRole("CanteenStaff"))
                return Forbid();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var (success, product, error) = await _productService.CreateAsync(dto);
            if (!success)
                return BadRequest(error);

            return Ok(product);
        }
    }
}

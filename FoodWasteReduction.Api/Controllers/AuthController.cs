using FoodWasteReduction.Application.DTOs.Auth;
using FoodWasteReduction.Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FoodWasteReduction.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController(IAuthService authService) : ControllerBase
    {
        private readonly IAuthService _authService = authService;

        [HttpPost("register/student")]
        public async Task<IActionResult> RegisterStudent(RegisterStudentDTO model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var (success, error) = await _authService.RegisterStudentAsync(model);
            if (!success)
                return BadRequest(error);

            return Ok();
        }

        [HttpPost("register/canteenstaff")]
        public async Task<IActionResult> RegisterCanteenStaff(RegisterCanteenStaffDTO model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var (success, error) = await _authService.RegisterCanteenStaffAsync(model);
            if (!success)
                return BadRequest(error);

            return Ok();
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDTO model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var (success, response, error) = await _authService.LoginAsync(model);
            if (!success)
                return BadRequest(error);

            return Ok(response);
        }
    }
}

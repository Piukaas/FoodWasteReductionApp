using System.ComponentModel.DataAnnotations;
using FoodWasteReduction.Core.Constants;
using FoodWasteReduction.Core.DTOs.Auth;
using FoodWasteReduction.Core.Entities;
using FoodWasteReduction.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace FoodWasteReduction.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ApplicationDbContext _applicationDbContext;

        public AuthController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ApplicationDbContext applicationDbContext
        )
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _applicationDbContext = applicationDbContext;
        }

        [HttpPost("register/student")]
        public async Task<IActionResult> RegisterStudent(RegisterStudentDto model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var validationContext = new ValidationContext(model);
            var validationResults = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(
                model,
                validationContext,
                validationResults,
                true
            );

            if (!isValid)
            {
                foreach (var validationResult in validationResults)
                {
                    if (validationResult.ErrorMessage != null)
                    {
                        ModelState.AddModelError(string.Empty, validationResult.ErrorMessage);
                    }
                }
                return BadRequest(ModelState);
            }

            var user = new Student
            {
                UserName = model.Email,
                Email = model.Email,
                Name = model.Name,
                StudentNumber = model.StudentNumber,
                DateOfBirth = model.DateOfBirth,
                StudyCity = model.StudyCity,
            };

            using var transaction = await _applicationDbContext.Database.BeginTransactionAsync();
            try
            {
                var result = await _userManager.CreateAsync(user, model.Password);
                if (!result.Succeeded)
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                    return BadRequest(ModelState);
                }

                await _userManager.AddToRoleAsync(user, Roles.Student);
                _applicationDbContext.Students?.Add(user);
                await _applicationDbContext.SaveChangesAsync();

                await transaction.CommitAsync();
                return Ok();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        [HttpPost("register/canteenstaff")]
        public async Task<IActionResult> RegisterCanteenStaff(RegisterCanteenStaffDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = new CanteenStaff
            {
                UserName = model.Email,
                Email = model.Email,
                Name = model.Name,
                PersonnelNumber = model.PersonnelNumber,
                Location = model.Location,
            };

            using var transaction = await _applicationDbContext.Database.BeginTransactionAsync();
            try
            {
                var result = await _userManager.CreateAsync(user, model.Password);
                if (!result.Succeeded)
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                    return BadRequest(ModelState);
                }

                await _userManager.AddToRoleAsync(user, Roles.CanteenStaff);
                _applicationDbContext.CanteenStaff?.Add(user);
                await _applicationDbContext.SaveChangesAsync();

                await transaction.CommitAsync();
                return Ok();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _signInManager.PasswordSignInAsync(
                model.Email,
                model.Password,
                isPersistent: false,
                lockoutOnFailure: false
            );

            if (result.Succeeded)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user == null)
                {
                    ModelState.AddModelError(string.Empty, "User not found.");
                    return BadRequest(ModelState);
                }

                var roles = await _userManager.GetRolesAsync(user);

                return Ok(
                    new
                    {
                        user.Email,
                        user.Name,
                        Roles = roles,
                    }
                );
            }

            ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            return BadRequest(ModelState);
        }
    }
}

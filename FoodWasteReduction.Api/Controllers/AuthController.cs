using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using FoodWasteReduction.Core.Constants;
using FoodWasteReduction.Core.DTOs.Auth;
using FoodWasteReduction.Core.Entities;
using FoodWasteReduction.Core.Enums;
using FoodWasteReduction.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace FoodWasteReduction.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ApplicationDbContext _applicationDbContext;
        private readonly IConfiguration _configuration;

        public AuthController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ApplicationDbContext applicationDbContext,
            IConfiguration configuration
        )
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _applicationDbContext = applicationDbContext;
            _configuration = configuration;
        }

        [HttpPost("register/student")]
        public async Task<IActionResult> RegisterStudent(RegisterStudentDTO model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Validate model
            var validationContext = new ValidationContext(model);
            var validationResults = new List<ValidationResult>();
            if (!Validator.TryValidateObject(model, validationContext, validationResults, true))
            {
                foreach (var validationResult in validationResults)
                {
                    if (validationResult.ErrorMessage != null)
                        ModelState.AddModelError(string.Empty, validationResult.ErrorMessage);
                }
                return BadRequest(ModelState);
            }

            using var transaction = await _applicationDbContext.Database.BeginTransactionAsync();
            try
            {
                // Create identity user
                var identityUser = new ApplicationUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                    Name = model.Name,
                    PhoneNumber = model.PhoneNumber,
                };

                var result = await _userManager.CreateAsync(identityUser, model.Password);
                if (!result.Succeeded)
                {
                    foreach (var error in result.Errors)
                        ModelState.AddModelError(string.Empty, error.Description);
                    return BadRequest(ModelState);
                }

                await _userManager.AddToRoleAsync(identityUser, Roles.Student);

                // Create domain entity
                var student = new Student
                {
                    Id = identityUser.Id,
                    StudentNumber = model.StudentNumber,
                    DateOfBirth = model.DateOfBirth,
                    StudyCity = model.StudyCity,
                };

                _applicationDbContext.Students?.Add(student);
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
        public async Task<IActionResult> RegisterCanteenStaff(RegisterCanteenStaffDTO model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            using var transaction = await _applicationDbContext.Database.BeginTransactionAsync();
            try
            {
                // Create identity user
                var identityUser = new ApplicationUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                    Name = model.Name,
                };

                var result = await _userManager.CreateAsync(identityUser, model.Password);
                if (!result.Succeeded)
                {
                    foreach (var error in result.Errors)
                        ModelState.AddModelError(string.Empty, error.Description);
                    return BadRequest(ModelState);
                }

                await _userManager.AddToRoleAsync(identityUser, Roles.CanteenStaff);

                // Create domain entity
                var canteenStaff = new CanteenStaff
                {
                    Id = identityUser.Id,
                    PersonnelNumber = model.PersonnelNumber,
                    Location = model.Location,
                };

                _applicationDbContext.CanteenStaff?.Add(canteenStaff);
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
        public async Task<IActionResult> Login(LoginDTO model)
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
                var token = GenerateJwtToken(user, roles);

                var responseData = new
                {
                    Token = token,
                    user.Id,
                    user.Email,
                    user.Name,
                    Roles = roles,
                };

                var additionalData = new Dictionary<string, object>();

                if (roles.Contains("Student"))
                {
                    var student = await _applicationDbContext.Students?.FirstOrDefaultAsync(s =>
                        s.Id == user.Id
                    )!;
                    if (student != null)
                    {
                        additionalData.Add("DateOfBirth", student.DateOfBirth);
                        additionalData.Add("StudyCity", student.StudyCity);
                    }
                }
                else if (roles.Contains("CanteenStaff"))
                {
                    var staff = await _applicationDbContext.CanteenStaff?.FirstOrDefaultAsync(cs =>
                        cs.Id == user.Id
                    )!;
                    if (staff != null)
                    {
                        additionalData.Add("Location", staff.Location);
                    }
                }

                return Ok(new { responseData, AdditionalData = additionalData });
            }

            ModelState.AddModelError(string.Empty, "Verkeerde login gegevens");
            return BadRequest(ModelState);
        }

        private string GenerateJwtToken(ApplicationUser user, IList<string> roles)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Name, user.UserName ?? string.Empty),
            };

            claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

            var jwtKey = _configuration["Jwt:Key"];
            if (string.IsNullOrEmpty(jwtKey))
            {
                throw new InvalidOperationException("JWT Key is not configured.");
            }
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(
                    Convert.ToDouble(_configuration["Jwt:ExpireMinutes"])
                ),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}

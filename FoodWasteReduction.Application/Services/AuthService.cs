using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using FoodWasteReduction.Application.DTOs.Auth;
using FoodWasteReduction.Application.Services.Interfaces;
using FoodWasteReduction.Core.Constants;
using FoodWasteReduction.Core.Entities;
using FoodWasteReduction.Core.Interfaces.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace FoodWasteReduction.Application.Services
{
    public class AuthService(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        IStudentRepository studentRepository,
        ICanteenStaffRepository canteenStaffRepository,
        IConfiguration configuration
    ) : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager = userManager;
        private readonly SignInManager<ApplicationUser> _signInManager = signInManager;
        private readonly IStudentRepository _studentRepository = studentRepository;
        private readonly ICanteenStaffRepository _canteenStaffRepository = canteenStaffRepository;
        private readonly IConfiguration _configuration = configuration;

        public async Task<(bool success, string? error)> RegisterStudentAsync(
            RegisterStudentDTO model
        )
        {
            var identityUser = new ApplicationUser
            {
                UserName = model.Email,
                Email = model.Email,
                Name = model.Name,
                PhoneNumber = model.PhoneNumber,
            };

            var result = await _userManager.CreateAsync(identityUser, model.Password);
            if (!result.Succeeded)
                return (false, result.Errors.First().Description);

            await _userManager.AddToRoleAsync(identityUser, Roles.Student);

            var student = new Student
            {
                Id = identityUser.Id,
                StudentNumber = model.StudentNumber,
                DateOfBirth = model.DateOfBirth,
                StudyCity = model.StudyCity,
                Email = model.Email,
                Name = model.Name,
                PhoneNumber = model.PhoneNumber,
            };

            await _studentRepository.CreateAsync(student);
            return (true, null);
        }

        public async Task<(bool success, string? error)> RegisterCanteenStaffAsync(
            RegisterCanteenStaffDTO model
        )
        {
            var identityUser = new ApplicationUser
            {
                UserName = model.Email,
                Email = model.Email,
                Name = model.Name,
            };

            var result = await _userManager.CreateAsync(identityUser, model.Password);
            if (!result.Succeeded)
                return (false, result.Errors.First().Description);

            await _userManager.AddToRoleAsync(identityUser, Roles.CanteenStaff);

            var canteenStaff = new CanteenStaff
            {
                Id = identityUser.Id,
                PersonnelNumber = model.PersonnelNumber,
                CanteenId = model.CanteenId,
            };

            await _canteenStaffRepository.CreateAsync(canteenStaff);
            return (true, null);
        }

        public async Task<(bool success, object? response, string? error)> LoginAsync(
            LoginDTO model
        )
        {
            var result = await _signInManager.PasswordSignInAsync(
                model.Email,
                model.Password,
                isPersistent: false,
                lockoutOnFailure: false
            );

            if (!result.Succeeded)
                return (false, null, "Invalid login credentials");

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
                return (false, null, "User not found");

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

            if (roles.Contains(Roles.Student))
            {
                var student = await _studentRepository.GetByIdAsync(user.Id);
                if (student != null)
                {
                    additionalData.Add("DateOfBirth", student.DateOfBirth);
                    additionalData.Add("StudyCity", student.StudyCity);
                }
            }
            else if (roles.Contains(Roles.CanteenStaff))
            {
                var staff = await _canteenStaffRepository.GetByIdAsync(user.Id);
                if (staff != null)
                {
                    additionalData.Add("CanteenId", staff.CanteenId);
                }
            }

            return (true, new { responseData, additionalData }, null);
        }

        private string GenerateJwtToken(ApplicationUser user, IList<string> roles)
        {
            var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Sub, user.Id),
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new(ClaimTypes.Name, user.UserName ?? string.Empty),
            };

            claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(
                    _configuration["Jwt:Key"]
                        ?? throw new InvalidOperationException("JWT Key is not configured.")
                )
            );
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

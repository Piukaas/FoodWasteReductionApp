using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using FluentAssertions;
using FoodWasteReduction.Web.Services;
using Microsoft.AspNetCore.Http;
using Moq;

namespace FoodWasteReduction.Tests.Services.Web
{
    public class AuthGuardServiceTests
    {
        private readonly Mock<IHttpContextAccessor> _httpContextAccessor;
        private readonly Mock<ISession> _session;
        private readonly AuthGuardService _service;

        public AuthGuardServiceTests()
        {
            _session = new Mock<ISession>();
            _httpContextAccessor = new Mock<IHttpContextAccessor>();
            var context = new DefaultHttpContext { Session = _session.Object };
            _httpContextAccessor.Setup(x => x.HttpContext).Returns(context);

            _service = new AuthGuardService(_httpContextAccessor.Object);
        }

        [Fact]
        public void IsAuthenticated_WithValidToken_ReturnsTrue()
        {
            // Arrange
            var token = GenerateTestToken(["Student"]);
            var tokenBytes = System.Text.Encoding.UTF8.GetBytes(token);
            _session.Setup(s => s.TryGetValue("JWTToken", out tokenBytes)).Returns(true);

            // Act
            var result = _service.IsAuthenticated;

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void HasRole_WithMatchingRole_ReturnsTrue()
        {
            // Arrange
            var token = GenerateTestToken(["Admin"]);
            _service.SetToken(token);

            // Act
            var result = _service.HasRole("Admin");

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void ClearToken_RemovesTokenFromSessionAndMemory()
        {
            // Arrange
            var token = GenerateTestToken(["Student"]);
            _service.SetToken(token);

            // Act
            _service.ClearToken();

            // Assert
            _service.IsAuthenticated.Should().BeFalse();
            _session.Verify(s => s.Remove("JWTToken"), Times.Once);
        }

        private static string GenerateTestToken(string[] roles)
        {
            var claims = roles.Select(role => new Claim(ClaimTypes.Role, role)).ToList();
            var token = new JwtSecurityToken(claims: claims);
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}

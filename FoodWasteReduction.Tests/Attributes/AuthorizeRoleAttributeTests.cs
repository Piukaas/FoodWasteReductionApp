using FluentAssertions;
using FoodWasteReduction.Web.Attributes;
using FoodWasteReduction.Web.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace FoodWasteReduction.Tests.Attributes
{
    public class AuthorizeRoleAttributeTests
    {
        private readonly Mock<IAuthGuardService> _authGuardMock;
        private readonly AuthorizationFilterContext _context;
        private readonly AuthorizeRoleAttribute _attribute;

        public AuthorizeRoleAttributeTests()
        {
            _authGuardMock = new Mock<IAuthGuardService>();

            var httpContext = new DefaultHttpContext();
            var actionContext = new ActionContext(
                httpContext,
                new RouteData(),
                new Microsoft.AspNetCore.Mvc.Abstractions.ActionDescriptor()
            );

            var services = new ServiceCollection();
            services.AddSingleton(_authGuardMock.Object);
            httpContext.RequestServices = services.BuildServiceProvider();

            _context = new AuthorizationFilterContext(actionContext, new List<IFilterMetadata>());

            _attribute = new AuthorizeRoleAttribute("Admin");
        }

        [Fact]
        public void OnAuthorization_WhenNotAuthenticated_RedirectsToLogin()
        {
            // Arrange
            _authGuardMock.Setup(x => x.IsAuthenticated).Returns(false);

            // Act
            _attribute.OnAuthorization(_context);

            // Assert
            var result = _context.Result as RedirectToActionResult;
            result?.Should().NotBeNull();
            result?.ActionName?.Should().Be("Login");
            result?.ControllerName?.Should().Be("Auth");
        }

        [Fact]
        public void OnAuthorization_WhenAuthenticatedWithoutRole_ReturnsForbid()
        {
            // Arrange
            _authGuardMock.Setup(x => x.IsAuthenticated).Returns(true);
            _authGuardMock.Setup(x => x.HasRole(It.IsAny<string>())).Returns(false);

            // Act
            _attribute.OnAuthorization(_context);

            // Assert
            _context.Result.Should().BeOfType<ForbidResult>();
        }

        [Fact]
        public void OnAuthorization_WhenAuthenticatedWithRole_Passes()
        {
            // Arrange
            _authGuardMock.Setup(x => x.IsAuthenticated).Returns(true);
            _authGuardMock.Setup(x => x.HasRole("Admin")).Returns(true);

            // Act
            _attribute.OnAuthorization(_context);

            // Assert
            _context.Result.Should().BeNull();
        }

        [Fact]
        public void OnAuthorization_WithMultipleRoles_PassesWithAnyRole()
        {
            // Arrange
            var attribute = new AuthorizeRoleAttribute("Admin", "Staff");
            _authGuardMock.Setup(x => x.IsAuthenticated).Returns(true);
            _authGuardMock.Setup(x => x.HasRole("Staff")).Returns(true);
            _authGuardMock.Setup(x => x.HasRole("Admin")).Returns(true);

            // Act
            attribute.OnAuthorization(_context);

            // Assert
            _context.Result.Should().BeNull();
        }
    }
}

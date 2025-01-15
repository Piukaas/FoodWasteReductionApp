using FluentAssertions;
using FoodWasteReduction.Application.DTOs.Json;
using FoodWasteReduction.Core.Entities;
using FoodWasteReduction.Core.Enums;
using FoodWasteReduction.Web.Controllers;
using FoodWasteReduction.Web.Models;
using FoodWasteReduction.Web.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace FoodWasteReduction.Tests.Controllers.Web
{
    public class PackagesControllerTests : ControllerTestBase
    {
        private readonly Mock<IPackageService> _packageService;
        private readonly Mock<ICanteenService> _canteenService;
        private readonly PackagesController _controller;

        public PackagesControllerTests()
        {
            _packageService = new Mock<IPackageService>();
            _canteenService = new Mock<ICanteenService>();
            _controller = new PackagesController(_packageService.Object, _canteenService.Object)
            {
                ControllerContext = new ControllerContext { HttpContext = HttpContext },
            };
        }

        [Fact]
        public async Task Index_WithCityFilter_ReturnsFilteredPackages()
        {
            // Arrange
            var city = City.Breda;
            var packages = new List<JsonPackageDTO>
            {
                new() { Id = 1, City = city },
            };
            _packageService.Setup(s => s.GetAvailablePackages(city, null)).ReturnsAsync(packages);

            // Act
            var result = await _controller.Index(city);

            // Assert
            var viewResult = result.Should().BeOfType<ViewResult>().Subject;
            viewResult.Model.Should().BeEquivalentTo(packages);
            viewResult.ViewData["UserCity"].Should().Be(city);
        }

        [Fact]
        public async Task Reservations_WithValidUser_ReturnsUserPackages()
        {
            // Arrange
            var userId = "user1";
            SetupUserSession(userId, "Student");
            var packages = new List<JsonPackageDTO>
            {
                new() { Id = 1, ReservedById = userId },
            };
            _packageService.Setup(s => s.GetReservedPackages(userId)).ReturnsAsync(packages);

            // Act
            var result = await _controller.Reservations();

            // Assert
            var viewResult = result.Should().BeOfType<ViewResult>().Subject;
            viewResult.Model.Should().BeEquivalentTo(packages);
        }

        [Fact]
        public async Task Manage_WithCanteenStaff_FiltersByLocation()
        {
            // Arrange
            var location = Location.LA;
            var canteens = new List<JsonCanteenDTO>
            {
                new() { Id = 1, Location = location },
            };
            SetupUserSession("staff1", "CanteenStaff", 1);
            _canteenService.Setup(s => s.GetCanteens()).ReturnsAsync(canteens);

            // Act
            var result = await _controller.Manage();

            // Assert
            var viewResult = result.Should().BeOfType<ViewResult>().Subject;
            viewResult.ViewData["StaffCanteen"].Should().Be(1);
        }

        [Fact]
        public async Task Create_Post_WithValidModel_RedirectsToManage()
        {
            // Arrange
            var model = new PackageViewModel
            {
                Name = "Test Package",
                Price = 5.95m,
                Type = MealType.Warm,
            };

            // Act
            var result = await _controller.Create(model);

            // Assert
            var redirectResult = result.Should().BeOfType<RedirectToActionResult>().Subject;
            redirectResult.ActionName.Should().Be(nameof(PackagesController.Manage));
        }

        [Fact]
        public async Task Edit_WithValidId_ReturnsPackageForEditing()
        {
            // Arrange
            var package = new JsonPackageDTO
            {
                Id = 1,
                Name = "Test Package",
                Price = 5.95m,
                Type = MealType.Warm,
            };
            _packageService.Setup(s => s.GetPackage(1)).ReturnsAsync(package);

            // Act
            var result = await _controller.Edit(1);

            // Assert
            var viewResult = result.Should().BeOfType<ViewResult>().Subject;
            var model = viewResult.Model.Should().BeOfType<PackageViewModel>().Subject;
            model.Id.Should().Be(package.Id);
            model.Name.Should().Be(package.Name);
        }

        [Fact]
        public async Task Delete_WithValidId_RedirectsToManage()
        {
            // Arrange
            var id = 1;

            // Act
            var result = await _controller.Delete(id);

            // Assert
            var redirectResult = result.Should().BeOfType<RedirectToActionResult>().Subject;
            redirectResult.ActionName.Should().Be(nameof(PackagesController.Manage));
        }

        [Fact]
        public async Task CreateProduct_WithValidModel_ReturnsOkResult()
        {
            // Arrange
            var model = new ProductViewModel { Name = "Test Product" };
            var product = new JsonProductDTO { Id = 1, Name = "Test Product" };
            _packageService.Setup(s => s.CreateProduct(model)).ReturnsAsync(product);

            // Act
            var result = await _controller.CreateProduct(model);

            // Assert
            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            okResult.Value.Should().BeEquivalentTo(product);
        }
    }
}

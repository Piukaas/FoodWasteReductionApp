using FluentAssertions;
using FoodWasteReduction.Api.Controllers;
using FoodWasteReduction.Api.Repositories.Interfaces;
using FoodWasteReduction.Application.DTOs;
using FoodWasteReduction.Core.Entities;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace FoodWasteReduction.Tests.Controllers.Api
{
    public class ReservationControllerTests : ApiControllerTestBase
    {
        private readonly Mock<IPackageRepository> _packageRepository;
        private readonly Mock<IStudentRepository> _studentRepository;
        private readonly ReservationController _controller;

        public ReservationControllerTests()
        {
            _packageRepository = new Mock<IPackageRepository>();
            _studentRepository = new Mock<IStudentRepository>();

            _controller = new ReservationController(
                _packageRepository.Object,
                _studentRepository.Object,
                UserManager.Object
            );
            SetupController(_controller);
        }

        [Fact]
        public async Task ReservePackage_WithoutStudentRole_ReturnsForbidden()
        {
            // Arrange
            SetupUserRole("CanteenStaff", _controller);
            var dto = new ReservePackageDTO { PackageId = 1, UserId = "user1" };

            // Act
            var result = await _controller.ReservePackage(dto);

            // Assert
            result.Should().BeOfType<ForbidResult>();
        }

        [Fact]
        public async Task ReservePackage_WithNonExistentPackage_ReturnsNotFound()
        {
            // Arrange
            SetupUserRole("Student", _controller);
            var dto = new ReservePackageDTO { PackageId = 1, UserId = "user1" };
            _packageRepository
                .Setup(r => r.GetPackageWithDetailsAsync(1))
                .ReturnsAsync((Package?)null);

            // Act
            var result = await _controller.ReservePackage(dto);

            // Assert
            var notFound = result.Should().BeOfType<NotFoundObjectResult>().Subject;
            notFound.Value.Should().Be("Package not found");
        }

        [Fact]
        public async Task ReservePackage_WithAlreadyReservedPackage_ReturnsBadRequest()
        {
            // Arrange
            SetupUserRole("Student", _controller);
            var dto = new ReservePackageDTO { PackageId = 1, UserId = "user1" };
            var package = new Package { Id = 1, ReservedById = "otherUser" };
            _packageRepository.Setup(r => r.GetPackageWithDetailsAsync(1)).ReturnsAsync(package);

            // Act
            var result = await _controller.ReservePackage(dto);

            // Assert
            var badRequest = result.Should().BeOfType<BadRequestObjectResult>().Subject;
            var error = badRequest.Value.Should().BeOfType<ErrorResponse>().Subject;
            error.Code.Should().Be("ALREADY_RESERVED");
        }

        [Fact]
        public async Task ReservePackage_WithUnderage_ReturnsBadRequest()
        {
            // Arrange
            SetupUserRole("Student", _controller);
            var dto = new ReservePackageDTO { PackageId = 1, UserId = "user1" };
            var package = new Package
            {
                Id = 1,
                Is18Plus = true,
                PickupTime = DateTime.Now.AddDays(1),
            };
            var student = new Student { Id = "user1", DateOfBirth = DateTime.Now.AddYears(-17) };
            var user = new ApplicationUser { Id = "user1" };

            _packageRepository.Setup(r => r.GetPackageWithDetailsAsync(1)).ReturnsAsync(package);
            _studentRepository.Setup(r => r.GetByIdAsync("user1")).ReturnsAsync(student);
            UserManager.Setup(u => u.FindByIdAsync("user1")).ReturnsAsync(user);

            // Act
            var result = await _controller.ReservePackage(dto);

            // Assert
            var badRequest = result.Should().BeOfType<BadRequestObjectResult>().Subject;
            var error = badRequest.Value.Should().BeOfType<ErrorResponse>().Subject;
            error.Code.Should().Be("AGE_RESTRICTION");
        }

        [Fact]
        public async Task ReservePackage_WithDuplicateReservation_ReturnsBadRequest()
        {
            // Arrange
            SetupUserRole("Student", _controller);
            var dto = new ReservePackageDTO { PackageId = 1, UserId = "user1" };
            var package = new Package { Id = 1, PickupTime = DateTime.Now.AddDays(1) };
            var student = new Student { Id = "user1", DateOfBirth = DateTime.Now.AddYears(-20) };
            var user = new ApplicationUser { Id = "user1" };

            _packageRepository.Setup(r => r.GetPackageWithDetailsAsync(1)).ReturnsAsync(package);
            _packageRepository
                .Setup(r => r.HasReservationOnDateAsync("user1", package.PickupTime))
                .ReturnsAsync(true);
            _studentRepository.Setup(r => r.GetByIdAsync("user1")).ReturnsAsync(student);
            UserManager.Setup(u => u.FindByIdAsync("user1")).ReturnsAsync(user);

            // Act
            var result = await _controller.ReservePackage(dto);

            // Assert
            var badRequest = result.Should().BeOfType<BadRequestObjectResult>().Subject;
            var error = badRequest.Value.Should().BeOfType<ErrorResponse>().Subject;
            error.Code.Should().Be("DUPLICATE_RESERVATION");
        }

        [Fact]
        public async Task ReservePackage_WithValidRequest_ReturnsOkWithReservedPackage()
        {
            // Arrange
            SetupUserRole("Student", _controller);
            var dto = new ReservePackageDTO { PackageId = 1, UserId = "user1" };
            var package = new Package { Id = 1, PickupTime = DateTime.Now.AddDays(1) };
            var reservedPackage = new Package { Id = 1, ReservedById = "user1" };
            var student = new Student { Id = "user1", DateOfBirth = DateTime.Now.AddYears(-20) };
            var user = new ApplicationUser { Id = "user1" };

            _packageRepository.Setup(r => r.GetPackageWithDetailsAsync(1)).ReturnsAsync(package);
            _packageRepository
                .Setup(r => r.HasReservationOnDateAsync("user1", package.PickupTime))
                .ReturnsAsync(false);
            _packageRepository
                .Setup(r => r.ReservePackageAsync(package, "user1"))
                .ReturnsAsync(reservedPackage);
            _studentRepository.Setup(r => r.GetByIdAsync("user1")).ReturnsAsync(student);
            UserManager.Setup(u => u.FindByIdAsync("user1")).ReturnsAsync(user);

            // Act
            var result = await _controller.ReservePackage(dto);

            // Assert
            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            var returnedPackage = okResult.Value.Should().BeOfType<Package>().Subject;
            returnedPackage.ReservedById.Should().Be("user1");
        }
    }
}

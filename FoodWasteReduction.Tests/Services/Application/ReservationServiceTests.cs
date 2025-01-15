using FluentAssertions;
using FoodWasteReduction.Application.DTOs;
using FoodWasteReduction.Application.Services;
using FoodWasteReduction.Core.Entities;
using FoodWasteReduction.Core.Interfaces.Repositories;
using Moq;

namespace FoodWasteReduction.Tests.Services.Application
{
    public class ReservationServiceTests : ServiceTestBase
    {
        private readonly Mock<IPackageRepository> _packageRepository;
        private readonly Mock<IStudentRepository> _studentRepository;
        private readonly ReservationService _service;

        public ReservationServiceTests()
        {
            _packageRepository = new Mock<IPackageRepository>();
            _studentRepository = new Mock<IStudentRepository>();

            _service = new ReservationService(
                _packageRepository.Object,
                _studentRepository.Object,
                UserManager.Object
            );
        }

        [Fact]
        public async Task ReservePackageAsync_PackageNotFound_ReturnsError()
        {
            // Arrange
            var dto = new ReservePackageDTO { PackageId = 1, UserId = "user1" };
            _packageRepository
                .Setup(r => r.GetPackageWithDetailsAsync(1))
                .ReturnsAsync((Package?)null);

            // Act
            var (success, package, error) = await _service.ReservePackageAsync(dto);

            // Assert
            success.Should().BeFalse();
            package.Should().BeNull();
            error.Should().NotBeNull();
            error!.Code.Should().Be("NOT_FOUND");
        }

        [Fact]
        public async Task ReservePackageAsync_PackageAlreadyReserved_ReturnsError()
        {
            // Arrange
            var dto = new ReservePackageDTO { PackageId = 1, UserId = "user1" };
            var package = new Package { Id = 1, ReservedById = "otherUser" };

            _packageRepository.Setup(r => r.GetPackageWithDetailsAsync(1)).ReturnsAsync(package);

            // Act
            var (success, resultPackage, error) = await _service.ReservePackageAsync(dto);

            // Assert
            success.Should().BeFalse();
            resultPackage.Should().BeNull();
            error!.Code.Should().Be("ALREADY_RESERVED");
        }

        [Fact]
        public async Task ReservePackageAsync_UserNotFound_ReturnsError()
        {
            // Arrange
            var dto = new ReservePackageDTO { PackageId = 1, UserId = "user1" };
            var package = new Package { Id = 1 };

            _packageRepository.Setup(r => r.GetPackageWithDetailsAsync(1)).ReturnsAsync(package);
            UserManager.Setup(u => u.FindByIdAsync("user1")).ReturnsAsync((ApplicationUser?)null);

            // Act
            var (success, resultPackage, error) = await _service.ReservePackageAsync(dto);

            // Assert
            success.Should().BeFalse();
            resultPackage.Should().BeNull();
            error!.Code.Should().Be("NOT_FOUND");
            error.Message.Should().Be("User not found");
        }

        [Fact]
        public async Task ReservePackageAsync_StudentNotFound_ReturnsError()
        {
            // Arrange
            var dto = new ReservePackageDTO { PackageId = 1, UserId = "user1" };
            var package = new Package { Id = 1 };
            var user = new ApplicationUser { Id = "user1" };

            _packageRepository.Setup(r => r.GetPackageWithDetailsAsync(1)).ReturnsAsync(package);
            UserManager.Setup(u => u.FindByIdAsync("user1")).ReturnsAsync(user);
            _studentRepository.Setup(r => r.GetByIdAsync("user1")).ReturnsAsync((Student?)null);

            // Act
            var (success, resultPackage, error) = await _service.ReservePackageAsync(dto);

            // Assert
            success.Should().BeFalse();
            resultPackage.Should().BeNull();
            error!.Code.Should().Be("NOT_FOUND");
            error.Message.Should().Be("Student not found");
        }

        [Fact]
        public async Task ReservePackageAsync_UnderageForRestrictedPackage_ReturnsError()
        {
            // Arrange
            var dto = new ReservePackageDTO { PackageId = 1, UserId = "user1" };
            var package = new Package
            {
                Id = 1,
                Is18Plus = true,
                PickupTime = DateTime.Now.AddDays(1),
            };
            var user = new ApplicationUser { Id = "user1" };
            var student = new Student { Id = "user1", DateOfBirth = DateTime.Now.AddYears(-17) };

            SetupMocksForValidUser(user, student, package);

            // Act
            var (success, resultPackage, error) = await _service.ReservePackageAsync(dto);

            // Assert
            success.Should().BeFalse();
            resultPackage.Should().BeNull();
            error!.Code.Should().Be("AGE_RESTRICTION");
        }

        [Fact]
        public async Task ReservePackageAsync_DuplicateReservation_ReturnsError()
        {
            // Arrange
            var dto = new ReservePackageDTO { PackageId = 1, UserId = "user1" };
            var package = new Package { Id = 1, PickupTime = DateTime.Now.AddDays(1) };
            var user = new ApplicationUser { Id = "user1" };
            var student = new Student { Id = "user1", DateOfBirth = DateTime.Now.AddYears(-20) };

            SetupMocksForValidUser(user, student, package);
            _packageRepository
                .Setup(r => r.HasReservationOnDateAsync("user1", package.PickupTime))
                .ReturnsAsync(true);

            // Act
            var (success, resultPackage, error) = await _service.ReservePackageAsync(dto);

            // Assert
            success.Should().BeFalse();
            resultPackage.Should().BeNull();
            error!.Code.Should().Be("DUPLICATE_RESERVATION");
        }

        [Fact]
        public async Task ReservePackageAsync_ValidRequest_ReturnsReservedPackage()
        {
            // Arrange
            var dto = new ReservePackageDTO { PackageId = 1, UserId = "user1" };
            var package = new Package { Id = 1, PickupTime = DateTime.Now.AddDays(1) };
            var user = new ApplicationUser { Id = "user1" };
            var student = new Student { Id = "user1", DateOfBirth = DateTime.Now.AddYears(-20) };
            var reservedPackage = new Package { Id = 1, ReservedById = "user1" };

            SetupMocksForValidUser(user, student, package);
            _packageRepository
                .Setup(r => r.HasReservationOnDateAsync("user1", package.PickupTime))
                .ReturnsAsync(false);
            _packageRepository
                .Setup(r => r.ReservePackageAsync(package, "user1"))
                .ReturnsAsync(reservedPackage);

            // Act
            var (success, resultPackage, error) = await _service.ReservePackageAsync(dto);

            // Assert
            success.Should().BeTrue();
            resultPackage.Should().NotBeNull();
            error.Should().BeNull();
            resultPackage!.Id.Should().Be(1);
        }

        private void SetupMocksForValidUser(ApplicationUser user, Student student, Package package)
        {
            _packageRepository.Setup(r => r.GetPackageWithDetailsAsync(1)).ReturnsAsync(package);
            UserManager.Setup(u => u.FindByIdAsync("user1")).ReturnsAsync(user);
            _studentRepository.Setup(r => r.GetByIdAsync("user1")).ReturnsAsync(student);
        }
    }
}

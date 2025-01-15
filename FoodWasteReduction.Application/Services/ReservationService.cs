using FoodWasteReduction.Application.DTOs;
using FoodWasteReduction.Application.Services.Interfaces;
using FoodWasteReduction.Core.Entities;
using FoodWasteReduction.Core.Interfaces.Repositories;
using Microsoft.AspNetCore.Identity;

namespace FoodWasteReduction.Application.Services
{
    public class ReservationService(
        IPackageRepository packageRepository,
        IStudentRepository studentRepository,
        UserManager<ApplicationUser> userManager
    ) : IReservationService
    {
        private readonly IPackageRepository _packageRepository = packageRepository;
        private readonly IStudentRepository _studentRepository = studentRepository;
        private readonly UserManager<ApplicationUser> _userManager = userManager;

        public async Task<(
            bool success,
            PackageDTO? package,
            ErrorResponse? error
        )> ReservePackageAsync(ReservePackageDTO dto)
        {
            var package = await _packageRepository.GetPackageWithDetailsAsync(dto.PackageId);
            if (package == null)
                return (
                    false,
                    null,
                    new ErrorResponse { Code = "NOT_FOUND", Message = "Package not found" }
                );

            if (package.ReservedById != null)
                return (
                    false,
                    null,
                    new ErrorResponse
                    {
                        Code = "ALREADY_RESERVED",
                        Message = "Dit pakket is al gereserveerd",
                    }
                );

            var user = await _userManager.FindByIdAsync(dto.UserId);
            if (user == null)
                return (
                    false,
                    null,
                    new ErrorResponse { Code = "NOT_FOUND", Message = "User not found" }
                );

            var student = await _studentRepository.GetByIdAsync(dto.UserId);
            if (student == null)
                return (
                    false,
                    null,
                    new ErrorResponse { Code = "NOT_FOUND", Message = "Student not found" }
                );

            if (package.Is18Plus)
            {
                var isOldEnough = await CheckAgeRestrictionAsync(student, package.PickupTime);
                if (!isOldEnough)
                    return (
                        false,
                        null,
                        new ErrorResponse
                        {
                            Code = "AGE_RESTRICTION",
                            Message = "Je moet 18 jaar of ouder zijn op de ophaaldatum",
                        }
                    );
            }

            var hasExistingReservation = await _packageRepository.HasReservationOnDateAsync(
                dto.UserId,
                package.PickupTime
            );
            if (hasExistingReservation)
                return (
                    false,
                    null,
                    new ErrorResponse
                    {
                        Code = "DUPLICATE_RESERVATION",
                        Message = "Je hebt al een reservering op deze datum",
                    }
                );

            var reservedPackage = await _packageRepository.ReservePackageAsync(package, dto.UserId);
            return (true, new PackageDTO(reservedPackage), null);
        }

        private static Task<bool> CheckAgeRestrictionAsync(Student student, DateTime pickupTime)
        {
            var ageAtPickup = pickupTime.Year - student.DateOfBirth.Year;
            if (pickupTime.Date < student.DateOfBirth.Date.AddYears(ageAtPickup))
                ageAtPickup--;

            return Task.FromResult(ageAtPickup >= 18);
        }
    }
}

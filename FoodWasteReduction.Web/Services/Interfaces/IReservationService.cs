namespace FoodWasteReduction.Web.Services.Interfaces
{
    public interface IReservationService
    {
        Task<(bool success, string? errorMessage)> ReservePackage(int packageId, string userId);
    }
}

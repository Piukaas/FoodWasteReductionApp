using FoodWasteReduction.Application.DTOs.Json;

namespace FoodWasteReduction.Web.Services.Interfaces
{
    public interface ICanteenService
    {
        Task<IEnumerable<JsonCanteenDTO>> GetCanteens();
    }
}

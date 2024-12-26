using FoodWasteReduction.Core.Entities;

namespace FoodWasteReduction.Web.Services.Interfaces
{
    public interface ICanteenService
    {
        Task<IEnumerable<Canteen>> GetCanteens();
    }
}

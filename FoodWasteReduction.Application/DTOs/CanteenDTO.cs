using FoodWasteReduction.Core.Entities;
using FoodWasteReduction.Core.Enums;

namespace FoodWasteReduction.Application.DTOs
{
    public class CanteenDTO(Canteen canteen)
    {
        public int Id { get; init; } = canteen.Id;
        public City City { get; init; } = canteen.City;
        public Location Location { get; init; } = canteen.Location;
        public bool ServesWarmMeals { get; init; } = canteen.ServesWarmMeals;
    }
}

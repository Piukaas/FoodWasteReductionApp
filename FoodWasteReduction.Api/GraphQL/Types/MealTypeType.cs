using FoodWasteReduction.Core.Enums;

namespace FoodWasteReduction.Api.GraphQL.Types
{
    public class MealTypeType : EnumType<MealType>
    {
        protected override void Configure(IEnumTypeDescriptor<MealType> descriptor)
        {
            descriptor.Value(MealType.Bread).Name("Bread");
            descriptor.Value(MealType.WarmMeal).Name("WarmMeal");
            descriptor.Value(MealType.Drinks).Name("Drinks");
            descriptor.Value(MealType.Snacks).Name("Snacks");
        }
    }
}

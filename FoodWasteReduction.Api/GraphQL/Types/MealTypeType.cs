using FoodWasteReduction.Core.Enums;

namespace FoodWasteReduction.Api.GraphQL.Types
{
    public class MealTypeType : EnumType<MealType>
    {
        protected override void Configure(IEnumTypeDescriptor<MealType> descriptor)
        {
            descriptor.Value(MealType.Brood).Name("Brood");
            descriptor.Value(MealType.Warm).Name("Warm");
            descriptor.Value(MealType.Drankjes).Name("Drankjes");
            descriptor.Value(MealType.Snacks).Name("Snacks");
        }
    }
}

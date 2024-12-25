using FoodWasteReduction.Core.Enums;

namespace FoodWasteReduction.Api.GraphQL.Types
{
    public class CityType : EnumType<City>
    {
        protected override void Configure(IEnumTypeDescriptor<City> descriptor)
        {
            descriptor.Value(City.Breda).Name("Breda");
            descriptor.Value(City.DenBosch).Name("DenBosch");
            descriptor.Value(City.Tilburg).Name("Tilburg");
        }
    }
}

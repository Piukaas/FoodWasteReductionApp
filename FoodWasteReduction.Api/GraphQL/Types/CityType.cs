using FoodWasteReduction.Core.Enums;

namespace FoodWasteReduction.Api.GraphQL.Types
{
    public class CityType : EnumType<City>
    {
        protected override void Configure(IEnumTypeDescriptor<City> descriptor)
        {
            descriptor.Value(City.Breda).Name("Breda");
            descriptor.Value(City.Den_Bosch).Name("Den_Bosch");
            descriptor.Value(City.Tilburg).Name("Tilburg");
        }
    }
}

using FoodWasteReduction.Core.Entities;

namespace FoodWasteReduction.Api.GraphQL.Types
{
    public class PackageType : ObjectType<Package>
    {
        protected override void Configure(IObjectTypeDescriptor<Package> descriptor)
        {
            descriptor.Field(p => p.Id);
            descriptor.Field(p => p.Name);
            descriptor.Field(p => p.City);
            descriptor.Field(p => p.CanteenId);
            descriptor.Field(p => p.Canteen);
            descriptor.Field(p => p.Type);
            descriptor.Field(p => p.PickupTime);
            descriptor.Field(p => p.ExpiryTime);
            descriptor.Field(p => p.Price);
            descriptor.Field(p => p.Is18Plus);
            descriptor.Field(p => p.ReservedBy);
            descriptor.Field(p => p.Products);
        }
    }
}

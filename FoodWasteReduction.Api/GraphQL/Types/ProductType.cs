using FoodWasteReduction.Core.Entities;
using HotChocolate.Types;

namespace FoodWasteReduction.Api.GraphQL.Types
{
    public class ProductType : ObjectType<Product>
    {
        protected override void Configure(IObjectTypeDescriptor<Product> descriptor)
        {
            descriptor.Field(p => p.Id);
            descriptor.Field(p => p.Name);
            descriptor.Field(p => p.ContainsAlcohol);
            descriptor.Field(p => p.ImageUrl);
        }
    }
}

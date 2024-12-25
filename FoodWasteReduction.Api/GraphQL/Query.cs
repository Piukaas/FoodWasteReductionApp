using FoodWasteReduction.Core.Entities;
using FoodWasteReduction.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FoodWasteReduction.Api.GraphQL
{
    public class Query
    {
        [UseProjection]
        [HotChocolate.Data.UseFiltering]
        [HotChocolate.Data.UseSorting]
        public IQueryable<Package> GetPackages([Service] ApplicationDbContext context)
        {
            return context
                .Packages?.Include(p => p.Products)
                .Include(p => p.ReservedBy)
                .Include(p => p.Canteen)!;
        }
    }
}

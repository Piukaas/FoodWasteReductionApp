using FoodWasteReduction.Core.Entities;
using FoodWasteReduction.Infrastructure.Data;
using HotChocolate.Authorization;
using Microsoft.EntityFrameworkCore;

namespace FoodWasteReduction.Api.GraphQL
{
    [Authorize]
    public class Query()
    {
        [UseProjection]
        [HotChocolate.Data.UseFiltering]
        [HotChocolate.Data.UseSorting]
        [Authorize]
        public IQueryable<Package> GetPackages([Service] ApplicationDbContext context)
        {
            return context
                .Packages?.Include(p => p.Products)
                .Include(p => p.ReservedBy)
                .Include(p => p.Canteen)!;
        }

        [UseProjection]
        [HotChocolate.Data.UseFiltering]
        [HotChocolate.Data.UseSorting]
        [Authorize]
        public IQueryable<Product> GetProducts([Service] ApplicationDbContext context)
        {
            return context.Products!;
        }
    }
}

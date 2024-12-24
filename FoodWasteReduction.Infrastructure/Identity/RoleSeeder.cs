using Microsoft.AspNetCore.Identity;
using FoodWasteReduction.Core.Constants;

namespace FoodWasteReduction.Infrastructure.Identity
{
    public static class RoleSeeder
    {
        public static async Task SeedRoles(RoleManager<IdentityRole> roleManager)
        {
            if (!await roleManager.RoleExistsAsync(Roles.Student))
            {
                await roleManager.CreateAsync(new IdentityRole(Roles.Student));
            }
            if (!await roleManager.RoleExistsAsync(Roles.CanteenStaff))
            {
                await roleManager.CreateAsync(new IdentityRole(Roles.CanteenStaff));
            }
        }
    }
}
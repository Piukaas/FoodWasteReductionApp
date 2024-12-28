using FoodWasteReduction.Core.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace FoodWasteReduction.Infrastructure.Identity
{
    public class ApplicationIdentityDbContext(
        DbContextOptions<ApplicationIdentityDbContext> options
    ) : IdentityDbContext<ApplicationUser>(options) { }
}

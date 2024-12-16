using Microsoft.EntityFrameworkCore;
using FoodWasteReduction.Core.Entities;

namespace FoodWasteReduction.Infrastructure.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // Add your DbSet properties here later
        public DbSet<Student> Students { get; set; }
        // public DbSet<Package> Packages { get; set; }
    }
}
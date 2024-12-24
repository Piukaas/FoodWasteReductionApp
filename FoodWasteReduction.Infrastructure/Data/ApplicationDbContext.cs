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

        public DbSet<Student>? Students { get; set; }
        public DbSet<CanteenStaff>? CanteenStaff { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Student>(entity =>
            {
                entity.ToTable("Students");
            });

            builder.Entity<CanteenStaff>(entity =>
            {
                entity.ToTable("CanteenStaff");
            });
        }
    }
}
using FoodWasteReduction.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace FoodWasteReduction.Infrastructure.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        public DbSet<Student>? Students { get; set; }
        public DbSet<CanteenStaff>? CanteenStaff { get; set; }
        public DbSet<Canteen>? Canteens { get; set; }
        public DbSet<Product>? Products { get; set; }
        public DbSet<Package>? Packages { get; set; }

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

            builder.Entity<Package>(entity =>
            {
                entity
                    .HasOne(p => p.Canteen)
                    .WithMany()
                    .HasForeignKey(p => p.CanteenId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasMany(p => p.Products).WithMany();

                entity
                    .HasOne(p => p.ReservedBy)
                    .WithMany()
                    .HasForeignKey(p => p.ReservedById)
                    .OnDelete(DeleteBehavior.SetNull);
            });
        }
    }
}

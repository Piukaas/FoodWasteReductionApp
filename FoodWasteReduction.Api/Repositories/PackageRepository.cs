using FoodWasteReduction.Api.Repositories.Interfaces;
using FoodWasteReduction.Core.Entities;
using FoodWasteReduction.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FoodWasteReduction.Api.Repositories
{
    public class PackageRepository(ApplicationDbContext context) : IPackageRepository
    {
        private readonly ApplicationDbContext _context = context;

        public async Task<Package> CreatePackageAsync(Package package)
        {
            await _context.Packages!.AddAsync(package);
            await _context.SaveChangesAsync();
            return package;
        }

        public async Task DeletePackageAsync(Package package)
        {
            _context.Packages!.Remove(package);
            await _context.SaveChangesAsync();
        }

        public async Task<Package> UpdatePackageAsync(Package package)
        {
            _context.Packages!.Update(package);
            await _context.SaveChangesAsync();
            return package;
        }

        public async Task<Package?> GetPackageWithDetailsAsync(int packageId)
        {
            return await _context
                .Packages?.Include(p => p.Products)
                .FirstOrDefaultAsync(p => p.Id == packageId)!;
        }

        public async Task<bool> HasReservationOnDateAsync(string userId, DateTime date)
        {
            return await _context.Packages!.AnyAsync(p =>
                p.ReservedById == userId && p.PickupTime.Date == date.Date
            );
        }

        public async Task<Package?> GetByIdAsync(int id)
        {
            return await _context.Packages?.FirstOrDefaultAsync(c => c.Id == id)!;
        }

        public async Task<Package?> GetPackageWithProductsAsync(int packageId)
        {
            return await _context
                .Packages?.Include(p => p.Products)
                .FirstOrDefaultAsync(p => p.Id == packageId)!;
        }

        public async Task<Package> ReservePackageAsync(Package package, string userId)
        {
            package.ReservedById = userId;
            await _context.SaveChangesAsync();
            return package;
        }

        public async Task<IEnumerable<Package>> GetPackagesAsync()
        {
            return await _context
                .Packages!.Include(p => p.Products)
                .Include(p => p.ReservedBy)
                .Include(p => p.Canteen)
                .ToListAsync();
        }
    }
}

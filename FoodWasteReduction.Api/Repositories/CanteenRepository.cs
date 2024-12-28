using FoodWasteReduction.Api.Repositories.Interfaces;
using FoodWasteReduction.Core.Entities;
using FoodWasteReduction.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FoodWasteReduction.Api.Repositories
{
    public class CanteenRepository(ApplicationDbContext context) : ICanteenRepository
    {
        private readonly ApplicationDbContext _context = context;

        public async Task<Canteen?> GetByIdAsync(int id)
        {
            return await _context.Canteens?.FirstOrDefaultAsync(c => c.Id == id)!;
        }

        public async Task<Canteen> CreateAsync(Canteen canteen)
        {
            await _context.Canteens!.AddAsync(canteen);
            await _context.SaveChangesAsync();
            return canteen;
        }

        public async Task<IEnumerable<Canteen>> GetAllAsync()
        {
            return await _context.Canteens!.ToListAsync();
        }
    }
}

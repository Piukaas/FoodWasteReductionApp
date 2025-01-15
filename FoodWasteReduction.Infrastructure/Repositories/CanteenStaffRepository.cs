using FoodWasteReduction.Core.Entities;
using FoodWasteReduction.Core.Interfaces.Repositories;
using FoodWasteReduction.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FoodWasteReduction.Infrastructure.Repositories
{
    public class CanteenStaffRepository : ICanteenStaffRepository
    {
        private readonly ApplicationDbContext _context;

        public CanteenStaffRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<CanteenStaff?> GetByIdAsync(string id)
        {
            return await _context.CanteenStaff!.FirstOrDefaultAsync(cs => cs.Id == id);
        }

        public async Task<CanteenStaff> CreateAsync(CanteenStaff canteenStaff)
        {
            await _context.CanteenStaff!.AddAsync(canteenStaff);
            await _context.SaveChangesAsync();
            return canteenStaff;
        }

        public async Task<CanteenStaff?> GetCanteenStaffWithDetailsAsync(string userId)
        {
            return await _context.CanteenStaff!.FirstOrDefaultAsync(cs => cs.Id == userId);
        }
    }
}

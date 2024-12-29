using FoodWasteReduction.Api.Repositories.Interfaces;
using FoodWasteReduction.Core.Entities;
using FoodWasteReduction.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FoodWasteReduction.Api.Repositories
{
    public class StudentRepository : IStudentRepository
    {
        private readonly ApplicationDbContext _context;

        public StudentRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Student?> GetByIdAsync(string id)
        {
            return await _context.Students?.FirstOrDefaultAsync(s => s.Id == id)!;
        }
    }
}

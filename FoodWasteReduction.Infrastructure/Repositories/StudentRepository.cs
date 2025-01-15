using FoodWasteReduction.Core.Entities;
using FoodWasteReduction.Core.Interfaces.Repositories;
using FoodWasteReduction.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FoodWasteReduction.Infrastructure.Repositories
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
            return await _context.Students!.FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<Student> CreateAsync(Student student)
        {
            await _context.Students!.AddAsync(student);
            await _context.SaveChangesAsync();
            return student;
        }

        public async Task<Student?> GetStudentWithDetailsAsync(string userId)
        {
            return await _context.Students!.FirstOrDefaultAsync(s => s.Id == userId);
        }
    }
}

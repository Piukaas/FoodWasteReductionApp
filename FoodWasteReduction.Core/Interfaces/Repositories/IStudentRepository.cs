using FoodWasteReduction.Core.Entities;

namespace FoodWasteReduction.Core.Interfaces.Repositories
{
    public interface IStudentRepository
    {
        Task<Student?> GetByIdAsync(string id);
        Task<Student> CreateAsync(Student student);
        Task<Student?> GetStudentWithDetailsAsync(string userId);
    }
}

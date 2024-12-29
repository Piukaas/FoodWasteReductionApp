using FluentAssertions;
using FoodWasteReduction.Api.Repositories;
using FoodWasteReduction.Core.Entities;

namespace FoodWasteReduction.Tests.Repositories
{
    public class StudentRepositoryTests : RepositoryTestBase
    {
        private readonly StudentRepository _repository;

        public StudentRepositoryTests()
            : base()
        {
            _repository = new StudentRepository(Context);
        }

        private async Task<Student> CreateTestStudent()
        {
            var student = new Student
            {
                Id = "test-id",
                Name = "Test Student",
                StudentNumber = "S123456",
                StudyCity = Core.Enums.City.Breda,
            };

            await Context.Students!.AddAsync(student);
            await Context.SaveChangesAsync();

            return student;
        }

        [Fact]
        public async Task GetByIdAsync_ExistingStudent_ReturnsStudent()
        {
            // Arrange
            var testStudent = await CreateTestStudent();

            // Act
            var result = await _repository.GetByIdAsync(testStudent.Id);

            // Assert
            result.Should().NotBeNull();
            result!.Id.Should().Be(testStudent.Id);
            result.Name.Should().Be(testStudent.Name);
        }

        [Fact]
        public async Task GetByIdAsync_NonExistingStudent_ReturnsNull()
        {
            // Act
            var result = await _repository.GetByIdAsync("non-existing-id");

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task CreateAsync_ValidStudent_CreatesAndReturnsStudent()
        {
            // Arrange
            var student = new Student
            {
                Id = "new-id",
                Name = "New Student",
                StudentNumber = "S654321",
                StudyCity = Core.Enums.City.Tilburg,
            };

            // Act
            var result = await _repository.CreateAsync(student);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(student.Id);

            var savedStudent = await Context.Students!.FindAsync(student.Id);
            savedStudent.Should().NotBeNull();
            savedStudent!.Name.Should().Be(student.Name);
        }

        [Fact]
        public async Task GetStudentWithDetailsAsync_ExistingStudent_ReturnsStudentWithDetails()
        {
            // Arrange
            var testStudent = await CreateTestStudent();

            // Act
            var result = await _repository.GetStudentWithDetailsAsync(testStudent.Id);

            // Assert
            result.Should().NotBeNull();
            result!.Id.Should().Be(testStudent.Id);
            result.Name.Should().Be(testStudent.Name);
        }
    }
}

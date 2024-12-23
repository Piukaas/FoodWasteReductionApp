using FoodWasteReduction.Core.Entities;
using Xunit;

namespace FoodWasteReduction.Tests
{
    public class StudentTests
    {
        [Fact]
        public void CanCreateStudent()
        {
            // Arrange
            var student = new Student
            {
                Id = 1,
                Name = "John Doe",
                DateOfBirth = new DateTime(2000, 1, 1),
                StudentNumber = "S123456",
                Email = "john.doe@example.com",
                StudyCity = "New York",
                PhoneNumber = "123-456-7890"
            };

            // Act & Assert
            Assert.Equal(1, student.Id);
            Assert.Equal("John Doe", student.Name);
            Assert.Equal(new DateTime(2000, 1, 1), student.DateOfBirth);
            Assert.Equal("S123456", student.StudentNumber);
            Assert.Equal("john.doe@example.com", student.Email);
            Assert.Equal("New York", student.StudyCity);
            Assert.Equal("123-456-7890", student.PhoneNumber);
        }
    }
}
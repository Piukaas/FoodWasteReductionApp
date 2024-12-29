using System.ComponentModel.DataAnnotations;
using FluentAssertions;
using FoodWasteReduction.Core.Entities;
using FoodWasteReduction.Core.Enums;

namespace FoodWasteReduction.Tests.Models.Entities
{
    public class StudentTests
    {
        private static Student CreateValidStudent()
        {
            return new Student
            {
                Id = "test-id",
                Name = "John Doe",
                Email = "john@example.com",
                PhoneNumber = "1234567890",
                DateOfBirth = DateTime.Today.AddYears(-20),
                StudentNumber = "S123456",
                StudyCity = City.Breda,
            };
        }

        [Fact]
        public void Student_WithValidData_PassesValidation()
        {
            // Arrange
            var student = CreateValidStudent();

            // Act
            var context = new ValidationContext(student);
            var results = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(student, context, results, true);

            // Assert
            isValid.Should().BeTrue();
            results.Should().BeEmpty();
        }

        [Fact]
        public void Student_WithMissingRequiredFields_FailsValidation()
        {
            // Arrange
            var student = new Student();

            // Act
            var context = new ValidationContext(student);
            var results = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(
                student,
                context,
                results,
                validateAllProperties: true
            );

            // Assert
            isValid.Should().BeFalse();
            var memberNames = results.SelectMany(r => r.MemberNames).ToList();
            memberNames.Should().BeEquivalentTo(["StudentNumber"]);
        }

        [Theory]
        [InlineData(-15)]
        [InlineData(0)]
        [InlineData(1)]
        public void Student_WithInvalidAge_FailsValidation(int yearsToAdd)
        {
            // Arrange
            var student = CreateValidStudent();
            student.DateOfBirth = DateTime.Today.AddYears(yearsToAdd);

            // Act
            var validationResults = new List<ValidationResult>();
            var context = new ValidationContext(student);
            var isValid = Validator.TryValidateObject(
                student,
                context,
                validationResults,
                validateAllProperties: true
            );

            // Assert
            isValid.Should().BeFalse();
            var result = validationResults.Should().ContainSingle().Subject;
            result.ErrorMessage.Should().Contain("16 jaar");
        }

        [Fact]
        public void Student_DefaultValues_AreCorrect()
        {
            // Arrange & Act
            var student = new Student();

            // Assert
            student.Id.Should().BeEmpty();
            student.Name.Should().BeEmpty();
            student.Email.Should().BeEmpty();
            student.PhoneNumber.Should().BeEmpty();
            student.StudentNumber.Should().BeEmpty();
        }

        [Theory]
        [InlineData(City.Breda)]
        [InlineData(City.Tilburg)]
        public void Student_WithValidCity_PassesValidation(City city)
        {
            // Arrange
            var student = CreateValidStudent();
            student.StudyCity = city;

            // Act
            var context = new ValidationContext(student);
            var results = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(student, context, results, true);

            // Assert
            isValid.Should().BeTrue();
            results.Should().BeEmpty();
        }
    }
}

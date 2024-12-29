using FluentAssertions;
using FoodWasteReduction.Core.Entities;
using Microsoft.AspNetCore.Identity;

namespace FoodWasteReduction.Tests.Models.Entities
{
    public class ApplicationUserTests
    {
        private static ApplicationUser CreateValidUser()
        {
            return new ApplicationUser
            {
                Id = "test-id",
                Name = "John Doe",
                UserName = "john@example.com",
                Email = "john@example.com",
                PhoneNumber = "1234567890",
            };
        }

        [Fact]
        public void ApplicationUser_DefaultValues_AreCorrect()
        {
            // Arrange & Act
            var user = new ApplicationUser();

            // Assert
            user.Name.Should().BeEmpty();
            user.PhoneNumber.Should().BeEmpty();
            user.Email.Should().BeNull();
            user.UserName.Should().BeNull();
        }

        [Fact]
        public void ApplicationUser_PropertiesSetCorrectly()
        {
            // Arrange
            var user = CreateValidUser();
            const string name = "Jane Doe";
            const string phone = "0987654321";

            // Act
            user.Name = name;
            user.PhoneNumber = phone;

            // Assert
            user.Name.Should().Be(name);
            user.PhoneNumber.Should().Be(phone);
        }

        [Fact]
        public void ApplicationUser_InheritsFromIdentityUser()
        {
            // Arrange
            var user = new ApplicationUser();

            // Assert
            user.Should().BeAssignableTo<IdentityUser>();
        }
    }
}

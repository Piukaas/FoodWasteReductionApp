using System.Net;
using FluentAssertions;
using FoodWasteReduction.Web.Models.Auth;
using FoodWasteReduction.Web.Services;
using Moq;
using Moq.Protected;

namespace FoodWasteReduction.Tests.Services.Web
{
    public class AuthServiceTests : ServiceTestBase
    {
        private readonly AuthService _service;

        public AuthServiceTests()
        {
            _service = new AuthService(HttpClientFactory.Object);
        }

        [Fact]
        public async Task Login_WithValidCredentials_ReturnsSuccessAndToken()
        {
            // Arrange
            var loginModel = new LoginViewModel { Email = "test@test.com", Password = "password" };
            var response = new
            {
                responseData = new
                {
                    token = "test-token",
                    roles = new[] { "Student" },
                    id = "user-id",
                    email = "test@test.com",
                    name = "Test User",
                },
                additionalData = new { StudyCity = 1, DateOfBirth = DateTime.Now },
            };

            SetupHttpResponse(HttpStatusCode.OK, response);

            // Act
            var (success, token, _) = await _service.Login(loginModel);

            // Assert
            success.Should().BeTrue();
            token.Should().Be("test-token");
        }

        [Fact]
        public async Task RegisterStudent_WithValidModel_ReturnsSuccess()
        {
            // Arrange
            var model = new RegisterStudentViewModel
            {
                Email = "student@test.com",
                Name = "Test Student",
                StudentNumber = "S123456",
                Password = "Password123!",
                ConfirmPassword = "Password123!",
                DateOfBirth = DateTime.Now.AddYears(-20),
                StudyCity = Core.Enums.City.Breda,
                PhoneNumber = "0612345678",
            };
            SetupHttpResponse(HttpStatusCode.OK);

            // Act
            var result = await _service.RegisterStudent(model);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public async Task RegisterStudent_WithDuplicateEmail_ReturnsFalse()
        {
            // Arrange
            var model = new RegisterStudentViewModel { Email = "existing@test.com" };
            SetupHttpResponse(HttpStatusCode.Conflict);

            // Act
            var result = await _service.RegisterStudent(model);

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public async Task RegisterStudent_WithServerError_ReturnsFalse()
        {
            // Arrange
            var model = new RegisterStudentViewModel();
            SetupHttpResponse(HttpStatusCode.InternalServerError);

            // Act
            var result = await _service.RegisterStudent(model);

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public async Task RegisterStudent_VerifiesRequestContent()
        {
            // Arrange
            var model = new RegisterStudentViewModel
            {
                Email = "test@test.com",
                Password = "password123",
            };

            HttpRequestMessage? capturedRequest = null;
            MessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .Callback<HttpRequestMessage, CancellationToken>(
                    (request, _) => capturedRequest = request
                )
                .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK));

            // Act
            await _service.RegisterStudent(model);

            // Assert
            capturedRequest.Should().NotBeNull();
            capturedRequest!.Method.Should().Be(HttpMethod.Post);
            capturedRequest.RequestUri!.PathAndQuery.Should().Be("/api/Auth/register/student");
        }

        [Fact]
        public async Task RegisterCanteenStaff_WithValidModel_ReturnsSuccess()
        {
            // Arrange
            var model = new RegisterCanteenStaffViewModel
            {
                Email = "staff@test.com",
                Name = "Test Staff",
                PersonnelNumber = "P123456",
                Password = "Password123!",
                ConfirmPassword = "Password123!",
                CanteenId = 1,
            };
            SetupHttpResponse(HttpStatusCode.OK);

            // Act
            var result = await _service.RegisterCanteenStaff(model);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public async Task RegisterCanteenStaff_WithDuplicatePersonnelNumber_ReturnsFalse()
        {
            // Arrange
            var model = new RegisterCanteenStaffViewModel { PersonnelNumber = "P123456" };
            SetupHttpResponse(HttpStatusCode.Conflict);

            // Act
            var result = await _service.RegisterCanteenStaff(model);

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public async Task RegisterCanteenStaff_VerifiesRequestContent()
        {
            // Arrange
            var model = new RegisterCanteenStaffViewModel
            {
                Email = "staff@test.com",
                PersonnelNumber = "P123456",
                CanteenId = 1,
            };

            HttpRequestMessage? capturedRequest = null;
            MessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .Callback<HttpRequestMessage, CancellationToken>(
                    (request, _) => capturedRequest = request
                )
                .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK));

            // Act
            await _service.RegisterCanteenStaff(model);

            // Assert
            capturedRequest.Should().NotBeNull();
            capturedRequest!.Method.Should().Be(HttpMethod.Post);
            capturedRequest.RequestUri!.PathAndQuery.Should().Be("/api/Auth/register/canteenstaff");
        }
    }
}

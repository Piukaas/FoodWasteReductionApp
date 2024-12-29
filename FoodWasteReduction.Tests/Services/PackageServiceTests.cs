using System.Net;
using FluentAssertions;
using FoodWasteReduction.Core.Entities;
using FoodWasteReduction.Web.Models;
using FoodWasteReduction.Web.Services;
using FoodWasteReduction.Web.Services.Interfaces;
using Moq;
using Moq.Protected;

namespace FoodWasteReduction.Tests.Services
{
    public class PackageServiceTests : ServiceTestBase
    {
        private readonly PackageService _service;

        public PackageServiceTests()
        {
            _service = new PackageService(HttpClientFactory.Object, AuthGuardService.Object);
        }

        [Fact]
        public async Task CreatePackage_WithValidModel_ReturnsPackage()
        {
            // Arrange
            var model = new PackageViewModel { Name = "Test Package" };
            var package = new Package { Id = 1, Name = "Test Package" };
            SetupHttpResponse(HttpStatusCode.Created, package);

            // Act
            var result = await _service.CreatePackage(model);

            // Assert
            result.Should().NotBeNull();
            result.Name.Should().Be(model.Name);
        }

        [Fact]
        public async Task GetPackage_WithValidId_ReturnsPackage()
        {
            // Arrange
            var packageId = 1;
            var graphQLResponse = new GraphQLResponse<PackagesData>
            {
                Data = new PackagesData
                {
                    Packages = [new Package { Id = packageId, Name = "Test" }],
                },
            };
            SetupHttpResponse(HttpStatusCode.OK, graphQLResponse);

            // Act
            var result = await _service.GetPackage(packageId);

            // Assert
            result.Should().NotBeNull();
            result!.Id.Should().Be(packageId);
        }

        [Fact]
        public async Task GetAvailablePackages_WithCityFilter_SendsCorrectQuery()
        {
            // Arrange
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
                .ReturnsAsync(
                    new HttpResponseMessage(HttpStatusCode.OK)
                    {
                        Content = new StringContent("""{"data":{"packages":[]}}"""),
                    }
                );

            // Act
            await _service.GetAvailablePackages(Core.Enums.City.Breda);

            // Assert
            capturedRequest.Should().NotBeNull();
            var content = await capturedRequest!.Content!.ReadAsStringAsync();
            content.Should().Contain("city: { eq: Breda }");
            content.Should().Contain("reservedById: { eq: null }");
            content.Should().Contain("packages(");
            content.Should().Contain("products {");
            content.Should().Contain("canteen {");
        }

        [Fact]
        public async Task GetReservedPackages_ForUser_ReturnsPackages()
        {
            // Arrange
            var userId = "user123";
            var packages = new[]
            {
                new Package { Id = 1, Name = "Reserved Package" },
            };
            var graphQLResponse = new GraphQLResponse<PackagesData>
            {
                Data = new PackagesData { Packages = packages },
            };
            SetupHttpResponse(HttpStatusCode.OK, graphQLResponse);

            // Act
            var result = await _service.GetReservedPackages(userId);

            // Assert
            result.Should().HaveCount(1);
            result.First().Name.Should().Be("Reserved Package");
        }

        [Fact]
        public async Task DeletePackage_WithValidId_SendsDeleteRequest()
        {
            // Arrange
            var packageId = 1;
            SetupHttpResponse(HttpStatusCode.NoContent);

            // Act & Assert
            await _service.Invoking(s => s.DeletePackage(packageId)).Should().NotThrowAsync();
        }

        [Fact]
        public async Task UpdatePackage_WithValidModel_ReturnsUpdatedPackage()
        {
            // Arrange
            var packageId = 1;
            var model = new PackageViewModel { Name = "Updated Package" };
            var package = new Package { Id = packageId, Name = "Updated Package" };
            SetupHttpResponse(HttpStatusCode.OK, package);

            // Act
            var result = await _service.UpdatePackage(packageId, model);

            // Assert
            result.Should().NotBeNull();
            result.Name.Should().Be(model.Name);
        }

        [Fact]
        public async Task CreatePackage_WithoutToken_ThrowsUnauthorized()
        {
            // Arrange
            SetupUnauthorizedAccess();
            var model = new PackageViewModel { Name = "Test Package" };

            // Act & Assert
            var act = () => _service.CreatePackage(model);
            await act.Should().ThrowAsync<Exception>().WithMessage("Failed to create package");
        }
    }
}

using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Serialization;
using FoodWasteReduction.Core.Entities;
using FoodWasteReduction.Core.Enums;
using FoodWasteReduction.Web.Models;
using FoodWasteReduction.Web.Services.Interfaces;

namespace FoodWasteReduction.Web.Services
{
    public class PackageService(
        IHttpClientFactory httpClientFactory,
        IAuthGuardService authGuardservice
    ) : IPackageService
    {
        private readonly HttpClient _httpClient = httpClientFactory.CreateClient("API");
        private readonly IAuthGuardService _authGuardservice = authGuardservice;

        public async Task<Package> CreatePackage(PackageViewModel model)
        {
            var token = _authGuardservice.GetToken();
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                token
            );

            var response = await _httpClient.PostAsJsonAsync("api/Packages", model);
            if (!response.IsSuccessStatusCode)
                throw new Exception("Failed to create package");

            return await response.Content.ReadFromJsonAsync<Package>()
                ?? throw new Exception("Invalid response");
        }

        public async Task<Product> CreateProduct(ProductViewModel model)
        {
            var token = _authGuardservice.GetToken();
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                token
            );

            var response = await _httpClient.PostAsJsonAsync("api/Products", model);
            if (!response.IsSuccessStatusCode)
                throw new Exception("Failed to create product");

            return await response.Content.ReadFromJsonAsync<Product>()
                ?? throw new Exception("Invalid response");
        }

        public async Task<Package> UpdatePackage(int id, PackageViewModel model)
        {
            var token = _authGuardservice.GetToken();
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                token
            );

            var response = await _httpClient.PutAsJsonAsync($"api/Packages/{id}", model);
            if (!response.IsSuccessStatusCode)
                throw new Exception("Failed to update package");

            return await response.Content.ReadFromJsonAsync<Package>()
                ?? throw new Exception("Invalid response");
        }

        public async Task DeletePackage(int id)
        {
            var token = _authGuardservice.GetToken();
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                token
            );

            var response = await _httpClient.DeleteAsync($"api/Packages/{id}");
            if (!response.IsSuccessStatusCode)
                throw new Exception("Failed to delete package");
        }

        public async Task<IEnumerable<Package>> GetAvailablePackages(
            City? city = null,
            MealType? type = null
        )
        {
            var query =
                @"
                    query GetPackages {
                        packages(
                            where: {
                                reservedById: { eq: null }
                                "
                + (city.HasValue ? $", city: {{ eq: {city.Value} }}" : "")
                + @"
                                "
                + (type.HasValue ? $", type: {{ eq: {type.Value} }}" : "")
                + @"
                            }
                        ) {
                            id
                            name
                            city
                            type
                            pickupTime
                            expiryTime
                            price
                            is18Plus
                            products {
                                name
                                containsAlcohol
                                imageUrl
                            }
                            canteen {
                                city
                                location
                            }
                        }
                    }";

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                Converters = { new JsonStringEnumConverter() },
            };

            var response = await _httpClient.PostAsJsonAsync("graphql", new { query });
            var content = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<GraphQLResponse<PackagesData>>(
                content,
                options
            );

            if (result?.Data?.Packages == null)
                return [];

            return result.Data.Packages;
        }

        public async Task<IEnumerable<Package>> GetReservedPackages(string? userId = null)
        {
            var query =
                @"
                query GetReservedPackages($userId: String) {
                    packages(where: { reservedById: { eq: $userId } }) {
                        id
                        name
                        price
                        pickupTime
                        expiryTime
                        is18Plus
                        type
                        products {
                            name
                            containsAlcohol
                            imageUrl
                        }
                        canteen {
                            city
                            location
                        }
                    }
                }";

            var variables = new { userId };
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                Converters = { new JsonStringEnumConverter() },
            };

            var response = await _httpClient.PostAsJsonAsync("graphql", new { query, variables });
            var content = await response.Content.ReadAsStringAsync();

            var result = JsonSerializer.Deserialize<GraphQLResponse<PackagesData>>(
                content,
                options
            );
            var packages = result?.Data?.Packages ?? [];
            return packages;
        }

        public async Task<IEnumerable<Package>> GetPackagesForManagement(int? canteenId = null)
        {
            var query =
                $@"
                query GetPackagesForManagement {{
                    packages{(canteenId.HasValue ? $"(where: {{ canteenId: {{ eq: {canteenId} }} }})" : "")} {{
                        id
                        name
                        price
                        pickupTime
                        expiryTime
                        is18Plus
                        type
                        city
                        products {{
                            id
                            name
                            containsAlcohol
                            imageUrl
                        }}
                        canteen {{
                            id
                            city
                            location
                        }}
                        reservedBy {{
                            id
                            email
                            name
                            phoneNumber
                        }}
                    }}
                }}";

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                Converters = { new JsonStringEnumConverter() },
            };

            var response = await _httpClient.PostAsJsonAsync("graphql", new { query });
            var content = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<GraphQLResponse<PackagesData>>(
                content,
                options
            );
            var packages = result?.Data?.Packages ?? [];

            return packages.OrderBy(p => p.PickupTime);
        }

        public async Task<IEnumerable<Product>> GetProducts()
        {
            var query =
                @"
                query {
                    products {
                        id
                        name
                        containsAlcohol
                        imageUrl
                    }
                }";

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                Converters = { new JsonStringEnumConverter() },
            };

            var response = await _httpClient.PostAsJsonAsync("graphql", new { query });
            var content = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<GraphQLResponse<ProductsData>>(
                content,
                options
            );

            return result?.Data?.Products ?? [];
        }
    }

    public class GraphQLResponse<T>
    {
        public T? Data { get; set; }
    }

    public class PackagesData
    {
        public IEnumerable<Package>? Packages { get; set; }
    }

    public class ProductsData
    {
        public IEnumerable<Product>? Products { get; set; }
    }
}

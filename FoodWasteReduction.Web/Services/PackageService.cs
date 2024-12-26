using System.Text.Json;
using FoodWasteReduction.Core.Entities;
using FoodWasteReduction.Core.Enums;
using FoodWasteReduction.Web.Services.Interfaces;

namespace FoodWasteReduction.Web.Services
{
    public class PackageService(IHttpClientFactory httpClientFactory) : IPackageService
    {
        private readonly HttpClient _httpClient = httpClientFactory.CreateClient("API");

        public async Task<IEnumerable<Package>> GetAvailablePackages(
            City? city = null,
            MealType? type = null
        )
        {
            var query =
                @"
            query GetAvailablePackages($city: City, $mealType: MealType) {
                packages(where: {
                    and: [
                        { reservedById: { isNull: true } },
                        { city: { eq: $city } },
                        { type: { eq: $mealType } }
                    ]
                }) {
                    id
                    name
                    price
                    pickupTime
                    expiryTime
                    products {
                        name
                        containsAlcohol
                    }
                    canteen {
                        city
                        location
                    }
                }
            }";

            var variables = new { city = city?.ToString(), mealType = type?.ToString() };
            var response = await _httpClient.PostAsJsonAsync("graphql", new { query, variables });
            var content = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<GraphQLResponse<PackagesData>>(content);

            return result?.Data?.Packages ?? [];
        }

        public async Task<IEnumerable<Package>> GetReservedPackages(string? userId = null)
        {
            var query =
                @"
            query GetReservedPackages($userId: String) {
                packages(where: {
                    reservedById: { eq: $userId }
                }) {
                    id
                    name
                    price
                    pickupTime
                    expiryTime
                    products {
                        name
                        containsAlcohol
                    }
                    canteen {
                        city
                        location
                    }
                    reservedBy {
                        name
                        email
                    }
                }
            }";

            var variables = new { userId };
            var response = await _httpClient.PostAsJsonAsync("graphql", new { query, variables });
            var content = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<GraphQLResponse<PackagesData>>(content);

            return result?.Data?.Packages ?? [];
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

            var response = await _httpClient.PostAsJsonAsync("graphql", new { query });
            var content = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<GraphQLResponse<ProductsData>>(content);

            return result?.Data?.Products ?? [];
        }

        public Task<IEnumerable<Package>> GetPackages(City? city = null, MealType? type = null)
        {
            throw new NotImplementedException();
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

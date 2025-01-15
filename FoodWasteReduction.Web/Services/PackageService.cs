using System.Net.Http.Headers;
using FoodWasteReduction.Application.DTOs.Json;
using FoodWasteReduction.Core.Enums;
using FoodWasteReduction.Web.Models;
using FoodWasteReduction.Web.Services.GraphQL;
using FoodWasteReduction.Web.Services.GraphQL.Models;
using FoodWasteReduction.Web.Services.Interfaces;

namespace FoodWasteReduction.Web.Services
{
    public class PackageService(
        IHttpClientFactory httpClientFactory,
        IAuthGuardService authGuardService
    ) : GraphQLServiceBase(httpClientFactory, authGuardService), IPackageService
    {
        private readonly HttpClient _httpClient = httpClientFactory.CreateClient("API");
        private readonly IAuthGuardService _authGuardService = authGuardService;

        public async Task<JsonPackageDTO> CreatePackage(PackageViewModel model)
        {
            var token = _authGuardService.GetToken();
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                token
            );

            var response = await _httpClient.PostAsJsonAsync("api/Packages", model);
            if (!response.IsSuccessStatusCode)
                throw new Exception("Failed to create package");

            return await response.Content.ReadFromJsonAsync<JsonPackageDTO>()
                ?? throw new Exception("Invalid response");
        }

        public async Task<JsonProductDTO> CreateProduct(ProductViewModel model)
        {
            var token = _authGuardService.GetToken();
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                token
            );

            var response = await _httpClient.PostAsJsonAsync("api/Products", model);
            if (!response.IsSuccessStatusCode)
                throw new Exception("Failed to create product");

            return await response.Content.ReadFromJsonAsync<JsonProductDTO>()
                ?? throw new Exception("Invalid response");
        }

        public async Task<JsonPackageDTO> UpdatePackage(int id, PackageViewModel model)
        {
            var token = _authGuardService.GetToken();
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                token
            );

            var response = await _httpClient.PutAsJsonAsync($"api/Packages/{id}", model);
            if (!response.IsSuccessStatusCode)
                throw new Exception("Failed to update package");

            return await response.Content.ReadFromJsonAsync<JsonPackageDTO>()
                ?? throw new Exception("Invalid response");
        }

        public async Task DeletePackage(int id)
        {
            var token = _authGuardService.GetToken();
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                token
            );

            var response = await _httpClient.DeleteAsync($"api/Packages/{id}");
            if (!response.IsSuccessStatusCode)
                throw new Exception("Failed to delete package");
        }

        public async Task<JsonPackageDTO?> GetPackage(int id)
        {
            var query =
                @"
                query GetPackages($id: Int!) {
                    packages(where: { id: { eq: $id } }) {
                        id
                        name
                        type
                        city
                        pickupTime
                        expiryTime
                        price
                        is18Plus
                        canteenId
                        products {
                            id
                            name
                            containsAlcohol
                        }
                        canteen {
                            id
                            city
                            location
                        }
                    }
                }";

            var response = await ExecuteGraphQLRequest<GraphQLResponse<PackagesData>>(
                query,
                new { id }
            );

            return response?.Data?.Packages?.FirstOrDefault();
        }

        public async Task<IEnumerable<JsonPackageDTO>> GetAvailablePackages(
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

            var variables = new { city = city?.ToString(), type = type?.ToString() };

            var response = await ExecuteGraphQLRequest<GraphQLResponse<PackagesData>>(
                query,
                variables
            );
            return response?.Data?.Packages ?? [];
        }

        public async Task<IEnumerable<JsonPackageDTO>> GetReservedPackages(string? userId = null)
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
            var response = await ExecuteGraphQLRequest<GraphQLResponse<PackagesData>>(
                query,
                variables
            );
            return response?.Data?.Packages ?? [];
        }

        public async Task<IEnumerable<JsonPackageDTO>> GetPackagesForManagement(
            int? canteenId = null
        )
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

            var variables = new { canteenId };
            var response = await ExecuteGraphQLRequest<GraphQLResponse<PackagesData>>(
                query,
                variables
            );
            return (response?.Data?.Packages ?? []).OrderBy(p => p.PickupTime);
        }

        public async Task<IEnumerable<JsonProductDTO>> GetProducts()
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

            var response = await ExecuteGraphQLRequest<GraphQLResponse<ProductsData>>(query);
            return response?.Data?.Products ?? [];
        }
    }
}

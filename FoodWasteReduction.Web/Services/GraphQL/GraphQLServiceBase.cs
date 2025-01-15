using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Serialization;
using FoodWasteReduction.Web.Services.Interfaces;

namespace FoodWasteReduction.Web.Services.GraphQL
{
    public abstract class GraphQLServiceBase
    {
        protected readonly HttpClient HttpClient;
        protected readonly IAuthGuardService AuthGuardService;
        protected readonly JsonSerializerOptions JsonOptions;

        protected GraphQLServiceBase(
            IHttpClientFactory clientFactory,
            IAuthGuardService authGuardService
        )
        {
            HttpClient = clientFactory.CreateClient("API");
            AuthGuardService = authGuardService;
            JsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                Converters = { new JsonStringEnumConverter() },
            };
        }

        protected async Task<T?> ExecuteGraphQLRequest<T>(string query, object? variables = null)
        {
            var token = AuthGuardService.GetToken();
            HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                token
            );

            var request = new { query, variables };
            var response = await HttpClient.PostAsJsonAsync("graphql", request, JsonOptions);
            var content = await response.Content.ReadAsStringAsync();

            return JsonSerializer.Deserialize<T>(content, JsonOptions);
        }
    }
}

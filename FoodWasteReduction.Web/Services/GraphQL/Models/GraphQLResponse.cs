using FoodWasteReduction.Application.DTOs.Json;

namespace FoodWasteReduction.Web.Services.GraphQL.Models
{
    public class GraphQLResponse<T>
    {
        public T? Data { get; set; }
    }

    public class PackagesData
    {
        public IEnumerable<JsonPackageDTO>? Packages { get; set; }
    }

    public class ProductsData
    {
        public IEnumerable<JsonProductDTO>? Products { get; set; }
    }
}

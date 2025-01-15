using FoodWasteReduction.Application.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

namespace FoodWasteReduction.Application
{
    public static class Program
    {
        public static IServiceCollection AddApplicationLayer(this IServiceCollection services)
        {
            services.AddApplicationServices();
            return services;
        }
    }
}

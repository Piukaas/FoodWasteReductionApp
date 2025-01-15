using FoodWasteReduction.Application.Services;
using FoodWasteReduction.Application.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace FoodWasteReduction.Application.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<ICanteenService, CanteenService>();
            services.AddScoped<IPackageService, PackageService>();
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<IReservationService, ReservationService>();
            return services;
        }
    }
}

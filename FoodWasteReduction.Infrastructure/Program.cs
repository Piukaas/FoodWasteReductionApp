using FoodWasteReduction.Infrastructure.DependencyInjection;
using FoodWasteReduction.Infrastructure.Identity;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FoodWasteReduction.Infrastructure
{
    public static class Program
    {
        public static IServiceCollection AddInfrastructureLayer(
            this IServiceCollection services,
            IConfiguration configuration
        )
        {
            services
                .AddDatabaseServices(configuration)
                .AddAuthenticationServices(configuration)
                .AddInfrastructureServices();

            return services;
        }

        public static IApplicationBuilder UseInfrastructureLayer(this IApplicationBuilder app)
        {
            app.UseAuthentication();
            app.UseAuthorization();

            return app;
        }

        public static async Task InitializeInfrastructureAsync(this IApplicationBuilder app)
        {
            using var scope = app.ApplicationServices.CreateScope();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            await RoleSeeder.SeedRoles(roleManager);
        }
    }
}

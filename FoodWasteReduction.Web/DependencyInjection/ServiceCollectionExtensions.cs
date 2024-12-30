using FoodWasteReduction.Web.Services;
using FoodWasteReduction.Web.Services.Interfaces;

namespace FoodWasteReduction.Web.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IAuthGuardService, AuthGuardService>();
            services.AddScoped<IPackageService, PackageService>();
            services.AddScoped<ICanteenService, CanteenService>();
            services.AddScoped<IReservationService, ReservationService>();

            return services;
        }

        public static IServiceCollection AddHttpConfiguration(
            this IServiceCollection services,
            IConfiguration configuration,
            IWebHostEnvironment environment
        )
        {
            var baseUrl = environment.IsDevelopment()
                ? configuration["ApiSettings:BaseUrl:Development"]
                : configuration["ApiSettings:BaseUrl:Production"];

            if (string.IsNullOrEmpty(baseUrl))
                throw new InvalidOperationException("API base URL is not configured");

            services
                .AddHttpClient(
                    "API",
                    client =>
                    {
                        client.BaseAddress = new Uri(baseUrl);
                        client.DefaultRequestHeaders.Accept.Add(
                            new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue(
                                "application/json"
                            )
                        );
                    }
                )
                .ConfigurePrimaryHttpMessageHandler(
                    () =>
                        new HttpClientHandler
                        {
                            ServerCertificateCustomValidationCallback =
                                HttpClientHandler.DangerousAcceptAnyServerCertificateValidator,
                        }
                );

            return services;
        }

        public static IServiceCollection AddSessionConfiguration(this IServiceCollection services)
        {
            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });

            return services;
        }

        public static IServiceCollection AddAuthenticationConfiguration(
            this IServiceCollection services
        )
        {
            services
                .AddAuthentication("CookieAuth")
                .AddCookie(
                    "CookieAuth",
                    options =>
                    {
                        options.LoginPath = "/Auth/Login";
                        options.AccessDeniedPath = "/Auth/Forbidden";
                    }
                );

            return services;
        }
    }
}

using FoodWasteReduction.Api.GraphQL;
using FoodWasteReduction.Api.GraphQL.Types;
using Microsoft.OpenApi.Models;

namespace FoodWasteReduction.Api.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddSwaggerServices(this IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc(
                    "v1",
                    new OpenApiInfo { Title = "FoodWasteReduction API", Version = "v1" }
                );
                c.EnableAnnotations();

                c.AddSecurityDefinition(
                    "Bearer",
                    new OpenApiSecurityScheme
                    {
                        Description = "JWT Authorization header using the Bearer scheme",
                        Name = "Authorization",
                        In = ParameterLocation.Header,
                        Type = SecuritySchemeType.Http,
                        Scheme = "bearer",
                    }
                );

                c.AddSecurityRequirement(
                    new OpenApiSecurityRequirement
                    {
                        {
                            new OpenApiSecurityScheme
                            {
                                Reference = new OpenApiReference
                                {
                                    Type = ReferenceType.SecurityScheme,
                                    Id = "Bearer",
                                },
                            },
                            Array.Empty<string>()
                        },
                    }
                );
            });

            return services;
        }

        public static IServiceCollection AddGraphQLServices(this IServiceCollection services)
        {
            services
                .AddGraphQLServer()
                .AddAuthorization()
                .AddDefaultTransactionScopeHandler()
                .AddQueryType<Query>()
                .AddType<PackageType>()
                .AddType<ProductType>()
                .AddType<CityType>()
                .AddType<MealTypeType>()
                .AddFiltering()
                .AddSorting()
                .AddProjections();

            return services;
        }
    }
}

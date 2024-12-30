using System.Text;
using FoodWasteReduction.Api.GraphQL;
using FoodWasteReduction.Api.GraphQL.Types;
using FoodWasteReduction.Api.Repositories;
using FoodWasteReduction.Api.Repositories.Interfaces;
using FoodWasteReduction.Core.Entities;
using FoodWasteReduction.Infrastructure.Data;
using FoodWasteReduction.Infrastructure.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

namespace FoodWasteReduction.Api.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddDatabaseServices(
            this IServiceCollection services,
            IConfiguration configuration
        )
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    configuration.GetConnectionString("DefaultConnection"),
                    b => b.MigrationsAssembly("FoodWasteReduction.Infrastructure")
                )
            );

            services.AddDbContext<ApplicationIdentityDbContext>(options =>
                options.UseSqlServer(
                    configuration.GetConnectionString("IdentityConnection"),
                    b => b.MigrationsAssembly("FoodWasteReduction.Infrastructure")
                )
            );

            return services;
        }

        public static IServiceCollection AddAuthenticationServices(
            this IServiceCollection services,
            IConfiguration configuration
        )
        {
            services
                .AddIdentity<ApplicationUser, IdentityRole>(options =>
                {
                    options.SignIn.RequireConfirmedAccount = false;
                    options.Password.RequireDigit = false;
                    options.Password.RequireLowercase = false;
                    options.Password.RequireNonAlphanumeric = false;
                    options.Password.RequireUppercase = false;
                    options.Password.RequiredLength = 8;
                    options.Password.RequiredUniqueChars = 1;
                })
                .AddEntityFrameworkStores<ApplicationIdentityDbContext>()
                .AddDefaultTokenProviders();

            services
                .AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = configuration["Jwt:Issuer"],
                        ValidAudience = configuration["Jwt:Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(
                                configuration["Jwt:Key"] ?? throw new InvalidOperationException()
                            )
                        ),
                    };
                });

            return services;
        }

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

        public static IServiceCollection AddRepositoryServices(this IServiceCollection services)
        {
            services.AddScoped<IPackageRepository, PackageRepository>();
            services.AddScoped<ICanteenRepository, CanteenRepository>();
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<IStudentRepository, StudentRepository>();
            services.AddScoped<ICanteenStaffRepository, CanteenStaffRepository>();

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

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

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

if (builder.Environment.IsDevelopment())
{
    builder.Configuration.AddUserSecrets<Program>();
}

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        b => b.MigrationsAssembly("FoodWasteReduction.Infrastructure")
    )
);

builder.Services.AddDbContext<ApplicationIdentityDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("IdentityConnection"),
        b => b.MigrationsAssembly("FoodWasteReduction.Infrastructure")
    )
);

// Add Identity
builder
    .Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
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

// Add JWT Authentication
builder
    .Services.AddAuthentication(options =>
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
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(
                    builder.Configuration["Jwt:Key"] ?? throw new InvalidOperationException()
                )
            ),
        };
    });

// Add GraphQL server
builder
    .Services.AddGraphQLServer()
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

builder.Services.AddHttpContextAccessor();

// Add Swagger configuration
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "FoodWasteReduction API", Version = "v1" });
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

builder.Services.AddScoped<IPackageRepository, PackageRepository>();
builder.Services.AddScoped<ICanteenRepository, CanteenRepository>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IStudentRepository, StudentRepository>();
builder.Services.AddScoped<ICanteenStaffRepository, CanteenStaffRepository>();

var app = builder.Build();

// Seed roles at startup
using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    await RoleSeeder.SeedRoles(roleManager);
}

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "FoodWasteReduction API v1");
    c.RoutePrefix = string.Empty;
});

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapGraphQL();
app.MapControllers();

app.Run();

using FoodWasteReduction.Api.DependencyInjection;
using FoodWasteReduction.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);

if (builder.Environment.IsDevelopment())
{
    builder.Configuration.AddUserSecrets<Program>();
}

builder.Services.AddControllers();
builder.Services.AddHttpContextAccessor();

builder
    .Services.AddDatabaseServices(builder.Configuration)
    .AddAuthenticationServices(builder.Configuration)
    .AddSwaggerServices()
    .AddRepositoryServices()
    .AddGraphQLServices();

var app = builder.Build();

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

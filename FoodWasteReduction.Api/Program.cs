using FoodWasteReduction.Api.DependencyInjection;
using FoodWasteReduction.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

if (builder.Environment.IsDevelopment())
{
    builder.Configuration.AddUserSecrets<Program>();
}

builder.Services.AddControllers();
builder.Services.AddHttpContextAccessor();

builder
    .Services.AddInfrastructureLayer(builder.Configuration)
    .AddSwaggerServices()
    .AddGraphQLServices();

var app = builder.Build();

await app.InitializeInfrastructureAsync();

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
app.UseInfrastructureLayer();

app.MapGraphQL();
app.MapControllers();

app.Run();

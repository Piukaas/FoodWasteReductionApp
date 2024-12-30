using FoodWasteReduction.Web.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

if (builder.Environment.IsDevelopment())
{
    builder.Configuration.AddUserSecrets<Program>();
}

builder.Services.AddControllersWithViews();
builder.Services.AddHttpContextAccessor();

builder
    .Services.AddApplicationServices()
    .AddHttpConfiguration(builder.Configuration, builder.Environment)
    .AddSessionConfiguration()
    .AddAuthenticationConfiguration();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseStaticFiles();
app.UseSession();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(name: "default", pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

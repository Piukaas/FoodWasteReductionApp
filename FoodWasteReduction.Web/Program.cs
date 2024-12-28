using FoodWasteReduction.Web.Services;
using FoodWasteReduction.Web.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

if (builder.Environment.IsDevelopment())
{
    builder.Configuration.AddUserSecrets<Program>();
}

// Configure API client
var baseUrl = builder.Environment.IsDevelopment()
    ? builder.Configuration["ApiSettings:BaseUrl:Development"]
    : builder.Configuration["ApiSettings:BaseUrl:Production"];

if (string.IsNullOrEmpty(baseUrl))
{
    throw new InvalidOperationException("API base URL is not configured");
}

builder
    .Services.AddHttpClient(
        "API",
        client =>
        {
            client.BaseAddress = new Uri(baseUrl);
            client.DefaultRequestHeaders.Accept.Add(
                new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json")
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

builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IAuthGuardService, AuthGuardService>();
builder.Services.AddScoped<IPackageService, PackageService>();
builder.Services.AddScoped<ICanteenService, CanteenService>();
builder.Services.AddScoped<IReservationService, ReservationService>();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder
    .Services.AddAuthentication("CookieAuth")
    .AddCookie(
        "CookieAuth",
        options =>
        {
            options.LoginPath = "/Auth/Login";
            options.AccessDeniedPath = "/Auth/Forbidden";
        }
    );

var app = builder.Build();

// Configure the HTTP request pipeline.
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

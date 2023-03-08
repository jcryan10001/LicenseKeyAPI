using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using System.Net;
using LicenseKeyAPI.Data;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
// Add configuration
var configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .Build();
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("https://localhost:7272/") });

//builder.Services.AddHttpClient("LocalApi", client => client.BaseAddress = new Uri("https://localhost:7143"));
builder.Services.AddRazorPages();
builder.Services.Configure<LicenseKeyConfig>(configuration.GetSection("LicenseKeyConfig"));
builder.Services.AddServerSideBlazor();
builder.Services.AddSingleton<WeatherForecastService>();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAnyOrigin",
        builder => builder.AllowAnyOrigin());
});
builder.Services.AddControllersWithViews(opt => {
    opt.OutputFormatters.RemoveType<HttpNoContentOutputFormatter>();
}).AddJsonOptions(opt =>
{
    opt.JsonSerializerOptions.PropertyNamingPolicy = null;
}).AddControllersAsServices();

builder.Services.AddHttpClient("weatherapi", c =>
{
    c.BaseAddress = new Uri("http://api.weatherapi.com");
    c.DefaultRequestHeaders.Add("User-Agent", "PortTime");
    c.DefaultRequestHeaders.Add("Accept", "application/json");
    c.DefaultRequestHeaders.Add("key", "a1e2e92d456c4c12800171759230103");
});
builder.Services.AddSession(options => {
    options.IdleTimeout = TimeSpan.FromHours(2);
});
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(config => {
        config.ExpireTimeSpan = TimeSpan.FromHours(8);
        config.SlidingExpiration = true;
        config.Events = new CookieAuthenticationEvents
        {
            OnRedirectToLogin = ctx =>
            {
                if (ctx.Request.Path.StartsWithSegments("/api"))
                {
                    ctx.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                }
                else
                {
                    ctx.Response.Redirect(ctx.RedirectUri);
                }
                return Task.FromResult(0);
            },
            OnSigningIn = ctx =>
            {
                return Task.FromResult(0);
            }
        };
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    //app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}


app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();


app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
    endpoints.MapBlazorHub();
    endpoints.MapFallbackToPage("/_Host");
});

app.Run();

using System.Security.Claims;
using business_logic.Services;
using data_access.Helpers;
using data_access.Interfaces;
using data_access.Repositories;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Add connection helper to connect to the database
builder.Services.AddSingleton<SqlConnectionHelper>(provider =>
{
    var config = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .Build();

    var connectionString = config.GetSection("ConnectionString").Value;

    return new SqlConnectionHelper(connectionString!);
});

// Add users and books repositories and services
builder.Services.AddTransient<IUserRepository, UserRepository>();
builder.Services.AddTransient<IBookRepository, BookRepository>();
builder.Services.AddScoped<AccountService>();
builder.Services.AddScoped<LibraryService>();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.RequireAuthenticatedSignIn = false;
})
    .AddCookie(options =>
    {
        options.Cookie.Name = "AuthenticationCookie";
        options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
        options.SlidingExpiration = true;
        options.Cookie.HttpOnly = true;
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireUserName("Admin"));
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Library/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Library}/{action=Index}");

app.Run();

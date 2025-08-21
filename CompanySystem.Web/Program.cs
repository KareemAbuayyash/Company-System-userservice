using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using CompanySystem.Data.Context;
using CompanySystem.Business.Interfaces;
using CompanySystem.Business.Services;
using CompanySystem.Business.Messaging.Configuration;
using CompanySystem.Business.Messaging.Infrastructure;
using CompanySystem.Business.Messaging.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews(); // Add MVC support

// Add Entity Framework with MySQL
builder.Services.AddDbContext<CompanySystemDbContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        new MySqlServerVersion(new Version(8, 0, 21))
    ));

// Register business services
builder.Services.AddScoped<IDepartmentService, DepartmentService>();
builder.Services.AddScoped<IMainPageContentService, MainPageContentService>();
builder.Services.AddScoped<INoteService, NoteService>();

// Configure RabbitMQ settings
var rabbitMQSettings = new RabbitMQSettings();
builder.Configuration.GetSection("RabbitMQ").Bind(rabbitMQSettings);
builder.Services.AddSingleton(rabbitMQSettings);

// Register RabbitMQ services
builder.Services.AddSingleton<IRabbitMQConnectionFactory, RabbitMQConnectionFactory>();
builder.Services.AddScoped<IAuthenticationMessageClient, AuthenticationMessageClient>();

// Register RabbitMQ-based Auth API service (comment out HTTP client for now)
builder.Services.AddScoped<IAuthApiService, RabbitMQAuthApiService>();

// Fallback HTTP client for Auth API (keep as backup)
// builder.Services.AddHttpClient<IAuthApiService, AuthApiService>();

// Add Authentication
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.LogoutPath = "/Account/Logout";
        options.AccessDeniedPath = "/Account/AccessDenied";
        options.ExpireTimeSpan = TimeSpan.FromHours(8);
        options.SlidingExpiration = true;
        options.Cookie.HttpOnly = true;
        options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
    });

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

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");

app.Run();

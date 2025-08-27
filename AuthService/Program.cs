using Microsoft.EntityFrameworkCore;
using AuthService.Data;
using AuthService.Services;
using AuthService.Messaging.Configuration;
using AuthService.Messaging.Infrastructure;
using AuthService.Messaging.Handlers;
using AuthService.Messaging.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add Entity Framework with MySQL
builder.Services.AddDbContext<AuthDbContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        new MySqlServerVersion(new Version(8, 0, 21))
    ));

// Register authentication services
builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();

// Configure RabbitMQ settings (bind directly from configuration)
var rabbitMQSettings = builder.Configuration.GetSection("RabbitMQ").Get<RabbitMQSettings>() ?? throw new InvalidOperationException("RabbitMQ configuration section is missing.");
builder.Services.AddSingleton(rabbitMQSettings);

// Register RabbitMQ services
builder.Services.AddSingleton<IRabbitMQConnectionFactory, RabbitMQConnectionFactory>();
builder.Services.AddSingleton<IAuthenticationMessageHandler, AuthenticationMessageHandler>();

// Register background service for message handling
builder.Services.AddHostedService<AuthenticationMessageService>();

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowMainSystem", policy =>
    {
        policy.WithOrigins("https://localhost:7001", "http://localhost:5000") // Main system URLs
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

// Add logging
builder.Services.AddLogging();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("AllowMainSystem");

app.UseAuthorization();

app.MapControllers();

// Ensure database is created
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AuthDbContext>();
    context.Database.EnsureCreated();
}

app.Run();

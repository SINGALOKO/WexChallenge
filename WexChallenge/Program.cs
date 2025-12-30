using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using WexChallenge.Application;
using WexChallenge.Infrastructure;
using WexChallenge.Infrastructure.Persistence;
using WexChallenge.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();

// Add Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "WexChallenge Purchase API",
        Version = "v1",
        Description = "API for managing purchase transactions with currency conversion support using Treasury Reporting Rates of Exchange.",
        Contact = new OpenApiContact
        {
            Name = "Fabio Silva",
            Email = "fabio.silva@example.com"
        }
    });

    // Include XML comments for API documentation
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        options.IncludeXmlComments(xmlPath);
    }
});

// Add Application and Infrastructure layer services
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration.GetConnectionString("DefaultConnection"));

var app = builder.Build();

// Ensure database is created (skip for in-memory databases used in tests)
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    // Only call EnsureCreated for relational databases (SQLite)
    // In-memory databases don't need this and it causes issues with multiple providers
    if (dbContext.Database.ProviderName != "Microsoft.EntityFrameworkCore.InMemory")
    {
        dbContext.Database.EnsureCreated();
    }
}

// Configure the HTTP request pipeline
// Enable Swagger for all environments (for evaluation purposes)
app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "WexChallenge API v1");
    options.RoutePrefix = string.Empty; // Swagger at root URL
    options.DocumentTitle = "WexChallenge API";
});

// Global exception handler middleware
app.UseGlobalExceptionHandler();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

// Make the implicit Program class public for integration testing
public partial class Program { }

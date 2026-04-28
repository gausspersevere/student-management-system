
using Microsoft.EntityFrameworkCore;
using StudentMS.Api.Data;

var builder = WebApplication.CreateBuilder(args);



// Controllers — registers all classes ending in "Controller"
builder.Services.AddControllers();

// Swagger — generates interactive API docs at /swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new()
    {
        Title       = "Student Management System API",
        Version     = "v1",
        Description = "ASP.NET Core Web API for SIMS — refactored from PHP/MySQL"
    });
});

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? Environment.GetEnvironmentVariable("DATABASE_URL")
    ?? throw new InvalidOperationException(
        "No database connection string found. " +
        "Set ConnectionStrings__DefaultConnection in appsettings or DATABASE_URL env var.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(
        connectionString,
        new MySqlServerVersion(new Version(8, 0, 0)),
        mySqlOptions => mySqlOptions.EnableRetryOnFailure(3)   // retry if DB isn't ready
    )
);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy
            .AllowAnyOrigin()   // In production: restrict to your frontend URL
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
builder.WebHost.UseUrls($"http://0.0.0.0:{port}");

// ============================================================
// Build the application
// ============================================================
var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    try
    {
        db.Database.Migrate();
    }
    catch (Exception ex)
    {
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
        logger.LogWarning("Migration failed (may already be up-to-date): {Message}", ex.Message);
    }
}

// Swagger always available
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Student MS API v1");
    c.RoutePrefix = "swagger";   // Visit /swagger to test your API
});


// Enable CORS before routing
app.UseCors("AllowFrontend");

// Map controller routes automatically:
//   [Route("api/students")] → /api/students
//   [Route("api/lookups")]  → /api/lookups
app.MapControllers();

// Health check endpoint — Render uses this to verify your app is alive
app.MapGet("/health", () => Results.Ok(new { status = "healthy", timestamp = DateTime.UtcNow }));

app.Run();

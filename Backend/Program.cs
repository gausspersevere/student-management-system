// ============================================================
// Program.cs  — Application Entry Point
//
// PHP EQUIVALENT: This replaces config.php + Apache's auto-routing.
// In PHP, Apache automatically found your .php files and ran them.
// Here, YOU configure the request pipeline, database, and routes.
//
// This is where Render deployment config lives too.
// ============================================================

using Microsoft.EntityFrameworkCore;
using StudentMS.Api.Data;

var builder = WebApplication.CreateBuilder(args);

// ============================================================
// STEP 1: Register Services (Dependency Injection Container)
// Think of this as declaring what your app needs to function.
// ============================================================

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

// ============================================================
// STEP 2: Database Connection (MySQL via Entity Framework Core)
//
// PHP equivalent:
//   $conn = mysqli_connect("localhost", "root", "", "student_management_db");
//
// Here we read from environment variables (safe!) instead of
// hardcoded credentials. On Render, you set these in the dashboard.
// Locally, you set them in appsettings.Development.json.
// ============================================================

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? Environment.GetEnvironmentVariable("DATABASE_URL")
    ?? throw new InvalidOperationException(
        "No database connection string found. " +
        "Set ConnectionStrings__DefaultConnection in appsettings or DATABASE_URL env var.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(
        connectionString,
        ServerVersion.AutoDetect(connectionString),
        mySqlOptions => mySqlOptions.EnableRetryOnFailure(3)   // retry if DB isn't ready
    )
);

// ============================================================
// STEP 3: CORS (Cross-Origin Resource Sharing)
//
// This is REQUIRED because your HTML frontend (running on a
// different port or domain) will make fetch() calls to this API.
// PHP didn't need this because the HTML and PHP were on the SAME server.
// ============================================================
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

// ============================================================
// STEP 4: Configure the PORT for Render
//
// Render dynamically assigns a port via the PORT environment variable.
// Your API MUST listen on that port or Render will think it crashed.
// ============================================================
var port = Environment.GetEnvironmentVariable("PORT") ?? "5000";
builder.WebHost.UseUrls($"http://0.0.0.0:{port}");

// ============================================================
// Build the application
// ============================================================
var app = builder.Build();

// ============================================================
// STEP 5: Auto-apply migrations on startup
// This ensures your MySQL tables exist before requests come in.
// Equivalent to importing setup.sql in phpMyAdmin.
// ============================================================
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

// ============================================================
// STEP 6: Middleware Pipeline
// Requests flow through these in order — like PHP's include chain.
// ============================================================

// Swagger always available (you can restrict to dev-only if needed)
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Student MS API v1");
    c.RoutePrefix = "swagger";   // Visit /swagger to test your API
});

// Redirect HTTP → HTTPS (Render handles SSL termination)
app.UseHttpsRedirection();

// Enable CORS before routing
app.UseCors("AllowFrontend");

// Map controller routes automatically:
//   [Route("api/students")] → /api/students
//   [Route("api/lookups")]  → /api/lookups
app.MapControllers();

// Health check endpoint — Render uses this to verify your app is alive
app.MapGet("/health", () => Results.Ok(new { status = "healthy", timestamp = DateTime.UtcNow }));

app.Run();

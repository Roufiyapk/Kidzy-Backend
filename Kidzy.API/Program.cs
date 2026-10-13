using FluentValidation;
using Kidzy.API.Extensions;
using Kidzy.Application.Validators.Auth;
using Kidzy.Infrastructure;
using Kidzy.Infrastructure.Data;
using Kidzy.Infrastructure.Data.Seed;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Load secrets.json if it exists
builder.Configuration.AddJsonFile(
    "secrets.json",
    optional: true,
    reloadOnChange: false
);

// Controllers
builder.Services.AddControllers();

// FluentValidation
builder.Services.AddValidatorsFromAssemblyContaining<RegisterValidator>();

// Infrastructure
builder.Services.AddInfrastructure(builder.Configuration);

// JWT Authentication
builder.Services.AddJwtAuthentication(builder.Configuration);

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReact", policy =>
    {
        policy
            .WithOrigins(
                "http://localhost:5173",
                "http://kidzy-frontend-2026-roufiya.s3-website.ap-south-1.amazonaws.com"
            )
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Swagger
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// HTTPS
app.UseHttpsRedirection();

// CORS
app.UseCors("AllowReact");

// Authentication and Authorization
app.UseAuthentication();
app.UseAuthorization();

// Controllers
app.MapControllers();

// Database Migration and Seeding
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    var dbContext =
        services.GetRequiredService<ApplicationDbContext>();

    await dbContext.Database.MigrateAsync();

    await CategorySeeder.SeedAsync(dbContext);

    var adminSeeder =
        services.GetRequiredService<AdminSeeder>();

    await adminSeeder.SeedAsync();
}

app.Run();
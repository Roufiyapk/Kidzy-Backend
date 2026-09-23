using Kidzy.API.Extensions;
using Kidzy.Infrastructure;
using Kidzy.Infrastructure.Data;
using Kidzy.Infrastructure.Data.Seed;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi;

var builder = WebApplication.CreateBuilder(args);

// Controllers
builder.Services.AddControllers();

// Infrastructure
builder.Services.AddInfrastructure(
    builder.Configuration);

// JWT Authentication
builder.Services.AddJwtAuthentication(
    builder.Configuration);

// Swagger
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition(
        "Bearer",
        new OpenApiSecurityScheme
        {
            Name = "Authorization",

            Type = SecuritySchemeType.Http,

            Scheme = "bearer",

            BearerFormat = "JWT",

            In = ParameterLocation.Header,

            Description =
                "Enter your JWT token. Example: Bearer eyJ..."
        });

    options.AddSecurityRequirement(document =>
        new OpenApiSecurityRequirement
        {
            [
                new OpenApiSecuritySchemeReference(
                    "Bearer",
                    document)
            ] = []
        });
});

var app = builder.Build();


// Apply migrations and seed Admin
using (var scope = app.Services.CreateScope())
{
    // Get DbContext
    var dbContext =
        scope.ServiceProvider
            .GetRequiredService<ApplicationDbContext>();

    // Apply migrations
    await dbContext.Database.MigrateAsync();

    // Get AdminSeeder
    var adminSeeder =
        scope.ServiceProvider
            .GetRequiredService<AdminSeeder>();

    // Create Admin if it doesn't exist
    await adminSeeder.SeedAsync();
}


// Swagger
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();

    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// JWT Authentication
app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
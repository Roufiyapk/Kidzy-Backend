using FluentValidation;
using Kidzy.API.Extensions;
using Kidzy.Application.Validators.Auth;
using Kidzy.Infrastructure;
using Kidzy.Infrastructure.Data;
using Kidzy.Infrastructure.Data.Seed;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi;

var builder = WebApplication.CreateBuilder(args);

// SERVICES

builder.Services.AddControllers();

// FluentValidation
builder.Services.AddValidatorsFromAssemblyContaining<RegisterValidator>();

builder.Services.AddInfrastructure(
    builder.Configuration);

builder.Services.AddJwtAuthentication(
    builder.Configuration);


// SWAGGER

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


// DATABASE + SEEDING

using (var scope = app.Services.CreateScope())
{
    var dbContext =
        scope.ServiceProvider
            .GetRequiredService<ApplicationDbContext>();

    // Apply migrations
    await dbContext.Database.MigrateAsync();


    // Create Admin
    var adminSeeder =
        scope.ServiceProvider
            .GetRequiredService<AdminSeeder>();

    await adminSeeder.SeedAsync();


    // Create Categories + SubCategories
    await CategorySeeder.SeedAsync(
        dbContext);
}


// SWAGGER

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();

    app.UseSwaggerUI();
}


// MIDDLEWARE

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
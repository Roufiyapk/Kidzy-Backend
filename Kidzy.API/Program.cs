using FluentValidation;
using Kidzy.API.Extensions;
using Kidzy.Application.Validators.Auth;
using Kidzy.Infrastructure;
using Kidzy.Infrastructure.Data;
using Kidzy.Infrastructure.Data.Seed;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddValidatorsFromAssemblyContaining<RegisterValidator>();

builder.Services.AddInfrastructure(
    builder.Configuration
);

builder.Services.AddJwtAuthentication(
    builder.Configuration
);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReact", policy =>
    {
        policy
            .WithOrigins("http://localhost:5173")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("AllowReact");

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var dbContext =
        scope.ServiceProvider
            .GetRequiredService<ApplicationDbContext>();

    await dbContext.Database.MigrateAsync();

    await CategorySeeder.SeedAsync(dbContext);

    var adminSeeder =
        scope.ServiceProvider
            .GetRequiredService<AdminSeeder>();

    await adminSeeder.SeedAsync();
}

app.Run();
using Kidzy.Api.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Controllers
builder.Services.AddControllers();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Application Services
builder.Services.AddApplicationServices();

// Infrastructure Services
builder.Services.AddInfrastructureServices(
    builder.Configuration);

// JWT Authentication
builder.Services.AddJwtAuthentication(
    builder.Configuration);

var app = builder.Build();

// Swagger
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// HTTPS
app.UseHttpsRedirection();

// Authentication
app.UseAuthentication();

// Authorization
app.UseAuthorization();

// Map Controllers
app.MapControllers();

app.Run();
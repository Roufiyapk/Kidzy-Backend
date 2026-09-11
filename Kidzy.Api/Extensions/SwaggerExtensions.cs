using Microsoft.OpenApi;

namespace Kidzy.Api.Extensions
{
    public static class SwaggerExtensions
    {
        public static IServiceCollection AddSwaggerDocumentation(
            this IServiceCollection services)
        {
            services.AddSwaggerGen(options =>
            {
                // JWT Bearer Definition
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
                            "Enter your JWT token"
                    });

                // JWT Bearer Requirement
                options.AddSecurityRequirement(
                    document =>
                        new OpenApiSecurityRequirement
                        {
                            [
                                new OpenApiSecuritySchemeReference(
                                    "Bearer",
                                    document)
                            ] = []
                        });
            });

            return services;
        }
    }
}
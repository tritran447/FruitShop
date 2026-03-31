using Microsoft.OpenApi.Models;

namespace FruitShop.Api.Configuration
{
    public static class DocumentationConfig
    {
        public static void Configure(IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "FruitShop API",
                    Version = "v1",
                    Description = "API documentation for FruitShop"
                });

                var jwtScheme = new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    Description = "Enter your JWT token like: Bearer {your token}"
                };
                c.AddSecurityDefinition("Bearer", jwtScheme);

                var requirement = new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Id = "Bearer",
                                Type = ReferenceType.SecurityScheme
                            }
                        },
                        Array.Empty<string>()
                    }
                };
                c.AddSecurityRequirement(requirement);

                c.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());
            });
        }
    }
}

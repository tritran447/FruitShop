using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace FruitShop.Api.Configuration
{
    public static class AuthenticationConfigExtensions
    {
        public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration config)
        {
            var jwtSection = config.GetSection("Jwt");
            var key         = jwtSection["Key"]!;
            var issuer      = jwtSection["Issuer"]!;
            var audience    = jwtSection["Audience"]!;

            services
                .AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme    = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(options =>
                {
                    options.RequireHttpsMetadata    = false;
                    options.SaveToken               = true;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey         = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
                        ValidateIssuer           = true,
                        ValidIssuer              = issuer,
                        ValidateAudience         = true,
                        ValidAudience            = audience,
                        ValidateLifetime         = true
                    };
                });

            return services;
        }
    }
}

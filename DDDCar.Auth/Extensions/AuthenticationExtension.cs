using DDDCar.ApplicationSettings;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace DDDCar.Auth.Extensions;

public static class AuthenticationExtension
{
    public static IServiceCollection AddApplicationAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<KeycloakSettings>(configuration.GetSection("KeycloakSettings"));
        
        var ks = configuration.GetSection("KeycloakSettings").Get<KeycloakSettings>()
                          ?? throw new InvalidOperationException("Настройки keycloak не найдены");

        services.AddAuthentication(opts =>
            {
                opts.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                opts.DefaultChallengeScheme    = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(opts =>
            {
                opts.Authority            = ks.Authority;
                opts.RequireHttpsMetadata = false;         

                opts.Audience = ks.Audience;                

                opts.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer           = true,
                    ValidIssuer              = ks.Authority,
                    ValidateIssuerSigningKey = true,
                    ValidateAudience         = true,        
                    ValidAudience            = ks.Audience, 
                    ValidateLifetime         = true,
                };
            });
        
        return services;
    }
}
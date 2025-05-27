using Microsoft.OpenApi.Models;

namespace DDDCar.Auth.Extensions;

public static class SwaggerJwtExtension
{
    public static IServiceCollection AddSwaggerJwt(this IServiceCollection services)
    {
        services.AddSwaggerGen(c =>
        {
            var jwtBearerScheme = new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                },
                Scheme = "bearer",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.Http,
                BearerFormat = "JWT",
                Description = "Вставьте access-токен (JWT), полученный из Keycloak, без префикса «Bearer»."
            };

            c.AddSecurityDefinition("Bearer", jwtBearerScheme);

            c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    jwtBearerScheme,
                    Array.Empty<string>()
                }
            });

            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "DDDCar.Auth API",
                Version = "v1",
                Description = "API с авторизацией через Keycloak. Поддерживает пользователей (password grant) и сервисы (client_credentials)."
            });
        });

        return services;
    }
}
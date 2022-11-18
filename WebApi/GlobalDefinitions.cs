using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

namespace WebApi;

public static class GlobalDefinitions
{
    public const string KeyPath = "keys";

    public const string ConnectionString = "Default";

    public static readonly OpenApiInfo OpenApiInfo = new()
    {
        Version = "v1",
        Title = "Dolite",
        Description = "Dolite Templates"
    };

    public static readonly OpenApiSecurityScheme OpenApiSecurityScheme = new()
    {
        Description =
            "JWT Authorization header using the Bearer scheme. Example: 'Authorization: Bearer {token}'",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        Reference = new OpenApiReference
        {
            Type = ReferenceType.SecurityScheme,
            Id = "Bearer"
        }
    };

    public static readonly OpenApiSecurityRequirement OpenApiSecurityRequirement = new()
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    };

    public static TokenValidationParameters JwtBearerTokenValidationParameters(SecurityKey publicKey)
    {
        return new TokenValidationParameters
        {
            ValidTypes = new[] {"JWT"},
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = publicKey,
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            RequireExpirationTime = true
        };
    }
}
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace Common.Security;

public static class AuthenticationExtension
{
    /// <summary>Registra autenticação JWT, gerador de token e hasher de senha. Falha rápido se Jwt:SecretKey estiver vazio.</summary>
    public static IServiceCollection AddJwtAuthentication(
        this IServiceCollection services, IConfiguration configuration)
    {
        var secret = configuration[$"{JwtOptions.SectionName}:SecretKey"];
        ArgumentException.ThrowIfNullOrWhiteSpace(secret);

        var options = new JwtOptions
        {
            SecretKey = secret,
            Issuer = configuration[$"{JwtOptions.SectionName}:Issuer"] ?? string.Empty,
            Audience = configuration[$"{JwtOptions.SectionName}:Audience"] ?? string.Empty
        };
        if (int.TryParse(configuration[$"{JwtOptions.SectionName}:ExpirationMinutes"], out var minutes) && minutes > 0)
        {
            options.ExpirationMinutes = minutes;
        }

        services.AddSingleton(options);
        services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
        services.AddSingleton<IPasswordHasher, BCryptPasswordHasher>();

        services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(o =>
            {
                o.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret)),
                    ValidateIssuer = true,
                    ValidIssuer = options.Issuer,
                    ValidateAudience = true,
                    ValidAudience = options.Audience,
                    ClockSkew = TimeSpan.Zero
                };
            });

        services.AddAuthorization();
        return services;
    }
}

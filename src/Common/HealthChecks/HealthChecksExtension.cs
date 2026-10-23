using System.Net.Mime;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Common.HealthChecks;

public static class HealthChecksExtension
{
    /// <summary>Registra "self" (liveness); "sqlserver" e "rabbitmq" (readiness) conforme configuração.</summary>
    public static void AddBasicHealthChecks(this WebApplicationBuilder builder)
    {
        var checks = builder.Services.AddHealthChecks()
            .AddCheck("self", () => HealthCheckResult.Healthy(), tags: ["liveness"]);

        var connectionString = builder.Configuration.GetConnectionString("Default");
        if (!string.IsNullOrWhiteSpace(connectionString))
        {
            checks.AddSqlServer(connectionString, name: "sqlserver", tags: ["readiness"]);
        }

        if (builder.Configuration["Messaging:Provider"] == "RabbitMq")
        {
            var rabbitConnection = builder.Configuration["Messaging:ConnectionString"];
            if (!string.IsNullOrWhiteSpace(rabbitConnection))
            {
                checks.AddRabbitMQ(
                    sp => new RabbitMQ.Client.ConnectionFactory { Uri = new Uri(rabbitConnection) }.CreateConnection(),
                    name: "rabbitmq",
                    tags: ["readiness"]);
            }
        }
    }

    public static void UseBasicHealthChecks(this WebApplication app)
    {
        app.MapHealthChecks("/health/live", Options("liveness"));
        app.MapHealthChecks("/health/ready", Options("readiness"));
        app.MapHealthChecks("/health", Options(tag: null));
    }

    private static HealthCheckOptions Options(string? tag) => new()
    {
        Predicate = check => tag is null || check.Tags.Contains(tag),
        ResultStatusCodes =
        {
            [HealthStatus.Healthy] = StatusCodes.Status200OK,
            [HealthStatus.Degraded] = StatusCodes.Status200OK,
            [HealthStatus.Unhealthy] = StatusCodes.Status503ServiceUnavailable
        },
        ResponseWriter = async (ctx, report) =>
        {
            ctx.Response.ContentType = MediaTypeNames.Application.Json;
            await ctx.Response.WriteAsJsonAsync(new
            {
                status = report.Status.ToString(),
                checks = report.Entries.Select(e => new
                {
                    name = e.Key,
                    status = e.Value.Status.ToString(),
                    description = e.Value.Description,
                    error = e.Value.Exception?.Message,
                    durationMs = e.Value.Duration.TotalMilliseconds
                })
            });
        }
    };
}

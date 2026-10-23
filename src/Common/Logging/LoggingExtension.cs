using System.Diagnostics;
using Microsoft.AspNetCore.Builder;
using Serilog;
using Serilog.Events;
using Serilog.Exceptions;
using Serilog.Formatting.Compact;
using Serilog.Sinks.SystemConsole.Themes;

namespace Common.Logging;

public static class LoggingExtension
{
    // Exclui do log requisições 200 em /health (ruído de probes)
    private static readonly Func<LogEvent, bool> ExcludeHealthProbes = e =>
    {
        if (e.Level != LogEventLevel.Information)
        {
            return false;
        }

        e.Properties.TryGetValue("StatusCode", out var status);
        e.Properties.TryGetValue("Path", out var path);
        return status?.ToString() == "200"
            && (path?.ToString().Contains("/health", StringComparison.Ordinal) ?? false);
    };

    public static WebApplicationBuilder AddDefaultLogging(this WebApplicationBuilder builder)
    {
        builder.Host.UseSerilog((ctx, cfg) =>
        {
            cfg.ReadFrom.Configuration(ctx.Configuration)
               .Enrich.FromLogContext()
               .Enrich.WithMachineName()
               .Enrich.WithProperty("Application", builder.Environment.ApplicationName)
               .Enrich.WithProperty("Environment", builder.Environment.EnvironmentName)
               .Enrich.WithExceptionDetails()
               .Filter.ByExcluding(ExcludeHealthProbes);

            if (Debugger.IsAttached)
            {
                cfg.WriteTo.Console(theme: SystemConsoleTheme.Colored);
            }
            else
            {
                // JSON compacto para agregadores (Seq, ELK, Datadog)
                cfg.WriteTo.Console(new CompactJsonFormatter());
            }
        });

        return builder;
    }
}

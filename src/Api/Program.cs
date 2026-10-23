using Api.Endpoints.Sistema;
using Api.Extensions;
using Api.Middleware;
using Application;
using Asp.Versioning;
using Common.HealthChecks;
using Common.Logging;
using Common.Security;
using Domain;
using FluentValidation;
using Infrastructure;
using Infrastructure.Data;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.AddDefaultLogging();
builder.AddBasicHealthChecks();

builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.AddAuthorization();

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddDomainServices();

builder.Services.AddValidatorsFromAssembly(typeof(Application.DependencyInjection).Assembly);

// Enums serializados como string
builder.Services.ConfigureHttpJsonOptions(options =>
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter()));

builder.Services.AddSwaggerWithVersioning();

var app = builder.Build();

MigrarBancoSeConfigurado(app);

// Deve ser o primeiro middleware do pipeline
app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseSwaggerWithVersioning();
app.UseBasicHealthChecks();

app.UseAuthentication();
app.UseAuthorization();

var apiVersionSet = app.NewApiVersionSet()
    .HasApiVersion(new ApiVersion(1, 0))
    .ReportApiVersions()
    .Build();

var v1 = app.MapGroup("/api/v{version:apiVersion}")
    .WithApiVersionSet(apiVersionSet)
    .HasApiVersion(new ApiVersion(1, 0));

v1.MapSistemaEndpoints();

app.Run();

// Migração na subida: falha de banco é logada e a API continua (health/ready reporta)
static void MigrarBancoSeConfigurado(WebApplication app)
{
    var migrar = app.Configuration.GetValue<bool>("Database:MigrateOnStartup");
    var connectionString = app.Configuration.GetConnectionString("Default");

    if (!migrar || string.IsNullOrWhiteSpace(connectionString))
    {
        return;
    }

    var logger = app.Services.GetRequiredService<ILoggerFactory>().CreateLogger("DatabaseMigrator");

    try
    {
        DatabaseMigrator.Migrate(connectionString, logger);
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Não foi possível migrar o banco; a API continuará sem migração.");
    }
}

public partial class Program { }

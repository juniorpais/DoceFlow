# Contrato de tipos — Etapa 0

> Fonte da verdade para nomes, namespaces e assinaturas da base da arquitetura. Todo agente/dev que escreve código na Etapa 0 segue este contrato à risca para que projetos compilem juntos. Derivado de `../../ESPECIFICACOES-TECNICAS.md`.

## Projetos e referências

| Projeto                 | Namespace raiz   | Referencia                                   |
| ----------------------- | ---------------- | -------------------------------------------- |
| `src/Common`            | `Common`         | —                                            |
| `src/Domain`            | `Domain`         | Common                                       |
| `src/Application`       | `Application`    | Domain, Common                               |
| `src/Infrastructure`    | `Infrastructure` | Application, Domain, Common                  |
| `src/Api`               | `Api`            | Application, Infrastructure, Domain, Common  |
| `tests/Unit`            | `Unit`           | Application, Domain, Common                  |
| `tests/Integration`     | `Integration`    | Api                                          |
| `tests/Functional`      | `Functional`     | Api                                          |

Target `net8.0`, `Nullable` e `ImplicitUsings` ativados, `TreatWarningsAsErrors` (ver `Directory.Build.props`). Namespaces **file-scoped**. Classes de handler/repositório/strategy são `sealed`.

## Pacotes por projeto (versões para net8.0)

| Projeto        | Pacotes                                                                                                                                                   |
| -------------- | --------------------------------------------------------------------------------------------------------------------------------------------------------- |
| Common         | `FluentResults` 3.16.0, `Microsoft.AspNetCore.App` (FrameworkReference), `Serilog.AspNetCore` 8.0.3, `Serilog.Enrichers.Environment` 3.0.1, `Serilog.Exceptions` 8.4.0, `Serilog.Formatting.Compact` 3.0.0, `Microsoft.AspNetCore.Authentication.JwtBearer` 8.0.11, `BCrypt.Net-Next` 4.0.3, `AspNetCore.HealthChecks.SqlServer` 8.0.2, `AspNetCore.HealthChecks.Rabbitmq` 8.0.2 (opcional) |
| Domain         | `FluentResults` 3.16.0                                                                                                                                    |
| Application    | `FluentResults` 3.16.0, `FluentValidation` 11.11.0, `Microsoft.Extensions.DependencyInjection.Abstractions` 8.0.2, `Microsoft.Extensions.Logging.Abstractions` 8.0.2 |
| Infrastructure | `Dapper` 2.1.35, `Microsoft.Data.SqlClient` 5.2.2, `dbup-sqlserver` 5.0.41, `Polly` 8.4.2, `Microsoft.Extensions.Configuration.Abstractions` 8.0.0, `Microsoft.Extensions.Hosting.Abstractions` 8.0.1 |
| Api            | `FluentValidation.DependencyInjectionExtensions` 11.11.0, `Asp.Versioning.Http` 8.1.0, `Asp.Versioning.Mvc.ApiExplorer` 8.1.0, `Swashbuckle.AspNetCore` 6.9.0 |
| Unit           | `xunit` 2.9.2, `xunit.runner.visualstudio` 2.8.2, `Microsoft.NET.Test.Sdk` 17.11.1, `NSubstitute` 5.3.0, `FluentAssertions` 6.12.2, `Bogus` 35.6.1, `coverlet.collector` 6.0.2 |
| Integration/Functional | idem Unit + `Microsoft.AspNetCore.Mvc.Testing` 8.0.11                                                                                            |

## Common

```csharp
namespace Common.Results;
public static class Errors
{
    public const string TypeKey = "Type";
    public static IError Validation(string message);           // Type = "Validation"   → 422
    public static IError NotFound(string message);             // Type = "NotFound"     → 404
    public static IError Conflict(string message);             // Type = "Conflict"     → 409
    public static IError CapacidadeIndisponivel(string message, IReadOnlyList<string> sugestoes); // Type = "CapacidadeIndisponivel" → 409, metadata "Sugestoes"
    public static IError Pagamento(string message);            // Type = "Pagamento"    → 402
    public static IError Unauthorized(string message);         // Type = "Unauthorized" → 401
    public static IError Forbidden(string message);            // Type = "Forbidden"    → 403
    public static IError ValidationFromDictionary(IDictionary<string, string[]> errors);
}
```

```csharp
namespace Common.Security;
public interface IPasswordHasher { string Hash(string password); bool Verify(string password, string hash); }
public sealed class BCryptPasswordHasher : IPasswordHasher;
public sealed class JwtOptions { public string SecretKey; public string Issuer; public string Audience; public int ExpirationMinutes = 60; }  // seção "Jwt"
public interface IJwtTokenGenerator { string Generate(Guid userId, string name, IReadOnlyCollection<string> roles); }
public sealed class JwtTokenGenerator(JwtOptions options) : IJwtTokenGenerator;
public static class AuthenticationExtension { public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration); }
public static class Roles { public const string Cliente = "Cliente", Atendente = "Atendente", Confeiteiro = "Confeiteiro", Entregador = "Entregador", Gerente = "Gerente", Admin = "Admin"; }
```

```csharp
namespace Common.Logging;
public static class LoggingExtension { public static WebApplicationBuilder AddDefaultLogging(this WebApplicationBuilder builder); }

namespace Common.HealthChecks;
public static class HealthChecksExtension
{
    public static void AddBasicHealthChecks(this WebApplicationBuilder builder); // "self" (liveness); "sqlserver" (readiness) se ConnectionStrings:Default; "rabbitmq" (readiness) se Messaging:Provider == "RabbitMq"
    public static void UseBasicHealthChecks(this WebApplication app);          // /health/live, /health/ready, /health — JSON conforme spec §18.2
}

namespace Common.Exceptions;
public sealed class DomainException(string message) : Exception(message);
```

## Domain

```csharp
namespace Domain.Common;
public interface IDomainEvent { Guid EventId { get; } DateTime OccurredAt { get; } }
public interface ICriticalDomainEvent : IDomainEvent { }       // vai para Outbox (ADR-005)
public abstract record DomainEvent : IDomainEvent { public Guid EventId { get; } = Guid.NewGuid(); public DateTime OccurredAt { get; } = DateTime.UtcNow; }
public abstract class BaseEntity
{
    public Guid Id { get; protected set; }
    public IReadOnlyCollection<IDomainEvent> DomainEvents { get; }
    protected void AddDomainEvent(IDomainEvent domainEvent);
    public void ClearDomainEvents();
}

namespace Domain.Specifications;
public interface ISpecification<in T> { bool IsSatisfiedBy(T entity); }
public static class SpecificationExtensions { And, Or, Not }   // combinadores

namespace Domain.ValueObjects;
public readonly record struct Dinheiro(decimal Valor) { + - *, Zero, Result<Dinheiro> Create(decimal) (não negativo, 2 casas) }

namespace Domain;
public static class DependencyInjection { public static IServiceCollection AddDomainServices(this IServiceCollection services); }
```

## Application

```csharp
namespace Application.Abstractions;
public interface ICommand : ICommand<CommandResult> { }
public interface ICommand<TResult> { }
public readonly record struct CommandResult;
public interface IQuery<TResult> { }
public interface ICommandHandler<in TCommand, TResult> where TCommand : ICommand<TResult> { Task<Result<TResult>> HandleAsync(TCommand command, CancellationToken cancellationToken); }
public interface IQueryHandler<in TQuery, TResult> where TQuery : IQuery<TResult> { Task<Result<TResult>> HandleAsync(TQuery query, CancellationToken cancellationToken); }
public interface IDispatcher
{
    Task<Result<TResult>> Send<TResult>(ICommand<TResult> command, CancellationToken cancellationToken);
    Task<Result> Send(ICommand command, CancellationToken cancellationToken);
    Task<Result<TResult>> Send<TResult>(IQuery<TResult> query, CancellationToken cancellationToken);
}
public sealed class Dispatcher(IServiceProvider serviceProvider) : IDispatcher;   // reflexão conforme spec §3.3
public interface IEventPublisher { Task PublishAsync<TEvent>(TEvent @event, CancellationToken cancellationToken) where TEvent : IDomainEvent; }
public interface IMessageBus
{
    Task SendAsync<TCommand>(TCommand command, CancellationToken cancellationToken) where TCommand : ICommand;
    Task PublishAsync<T>(T message, string? sessionId, CancellationToken cancellationToken) where T : class;
}
public interface IDateTimeProvider { DateTime UtcNow { get; } }

namespace Application.Common;
public abstract record PagedQuery { public int Page { get; init; } = 1; public int Size { get; init; } = 10; public string? Order { get; init; } }
public sealed record PagedResult<T>(IReadOnlyList<T> Data, int TotalItems, int CurrentPage, int TotalPages) { public static PagedResult<T> Create(IReadOnlyList<T> data, int total, int page, int size); }
public abstract class PagedQueryValidator<T> : AbstractValidator<T> where T : PagedQuery;  // Page >= 1, Size 1..100

namespace Application.Features.Sistema;
public sealed record PingQuery : IQuery<PingResponse>;                      // feature mínima para validar o pipeline
public sealed record PingResponse(string Mensagem, DateTime ServidorUtc);
public sealed class PingHandler(IDateTimeProvider clock) : IQueryHandler<PingQuery, PingResponse>;

namespace Application;
public static class DependencyInjection { public static IServiceCollection AddApplication(this IServiceCollection services); } // registra IDispatcher (scoped) e PingHandler
```

## Infrastructure

```csharp
namespace Infrastructure.Data;
public interface IDbConnectionFactory { IDbConnection CreateConnection(); }
public sealed class SqlConnectionFactory(string connectionString) : IDbConnectionFactory;
public static class OrderByBuilder { public static string Build(string? order, IReadOnlyDictionary<string, string> allowed, string fallback); }
public static class DatabaseMigrator { public static void Migrate(string connectionString, ILogger logger); } // DbUp, scripts embutidos em Infrastructure/Data/Scripts/V###__nome.sql, cria o banco se não existir

namespace Infrastructure.Events;
public sealed class LogEventPublisher(ILogger<LogEventPublisher> logger) : IEventPublisher;
public sealed class LogMessageBus(ILogger<LogMessageBus> logger) : IMessageBus;
public sealed class SystemDateTimeProvider : IDateTimeProvider;

namespace Infrastructure;
public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration);
    // registra IDbConnectionFactory (singleton, ConnectionStrings:Default), IDateTimeProvider,
    // IEventPublisher/IMessageBus = Log quando Messaging:Provider vazio ou "Log" (RabbitMq/ServiceBus: lançar NotSupportedException com mensagem clara — implementação em etapa futura)
}
public sealed class MessagingOptions { public string Provider { get; set; } = "Log"; public string? ConnectionString { get; set; } }   // seção "Messaging"
```

Script inicial `V001__infra.sql` cria: `tblUsuarios`, `tblOtp`, `tblOutbox`, `tblIdempotencia`, `tblAuditoria` (colunas conforme `docs/07-modelo-de-dados.md`, seção "Tabelas de infraestrutura").

## Api

```csharp
namespace Api.Results;
public static class ResultExtensions
{
    public static IResult ToIResult<T>(this Result<T> result);        // 200 ou ProblemDetails por Errors.TypeKey
    public static IResult ToIResult(this Result result);              // 204 ou ProblemDetails
    public static IResult ToValidationResult(this ValidationResult validationResult); // Results.ValidationProblem
}

namespace Api.Middleware;
public sealed class ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger); // spec §17

namespace Api.Endpoints.Sistema;
public static class SistemaEndpoints { public static IEndpointRouteBuilder MapSistemaEndpoints(this IEndpointRouteBuilder app); } // GET /sistema/ping → PingQuery via IDispatcher
```

`Program.cs`: `public partial class Program { }` ao final (para `WebApplicationFactory<Program>`). Pipeline: `AddDefaultLogging` → `AddBasicHealthChecks` → `AddJwtAuthentication` → `AddApplication` → `AddInfrastructure` → `AddDomainServices` → validators → versioning + swagger → `UseMiddleware<ExceptionHandlingMiddleware>` (primeiro) → `UseBasicHealthChecks` → auth → `/api/v{version:apiVersion}` com `MapSistemaEndpoints`. Migração roda na subida quando `Database:MigrateOnStartup = true` (Development) e `ConnectionStrings:Default` presente.

Configuração: `appsettings.json` (sem segredos), `appsettings.Development.json` (`ConnectionStrings:Default` para `localhost,1433`, `Messaging:Provider = Log`, `Database:MigrateOnStartup = true`), `appsettings.Testing.json` (banco `DoceFlow_test`, `Messaging:Provider = Log`). `Jwt:SecretKey` só em Development/Testing com valor de desenvolvimento claramente marcado.

## Testes

- `tests/Unit`: `DispatcherTests` (resolve handler, lança se não registrado), `ErrorsTests`, `PagedResultTests`, `OrderByBuilderTests` (Unit referencia Infrastructure? **Não**: mover `OrderByBuilderTests` para Integration ou referenciar Infrastructure a partir de Unit — decisão: Unit referencia também Infrastructure para testar helpers puros), `DependencyInjectionTests` (todo `ICommandHandler<,>`/`IQueryHandler<,>` concreto no assembly Application está registrado), `BaseEntityTests`, `DinheiroTests`.
- `tests/Integration`: `CustomWebApplicationFactory` (ambiente `Testing`, remove `IHostedService`, substitui `IEventPublisher` por `LogEventPublisher`), `BaseIntegrationTest`, `HealthEndpointsTests` (`/health/live` 200), `SistemaEndpointsTests` (`/api/v1/sistema/ping` 200). Banco é opcional nesta etapa: testes que precisam de SQL Server são marcados com `[Trait("Category","Database")]` e pulam quando `ConnectionStrings:Default` vazio.
- Convenção de nome: `Given_{Estado}_When_{Acao}_Then_{Resultado}` + `DisplayName`.

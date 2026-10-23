using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace Api.Middleware;

/// <summary>Garante resposta ProblemDetails consistente para exceções não tratadas, sem vazar stack trace.</summary>
public sealed class ExceptionHandlingMiddleware(
    RequestDelegate next,
    ILogger<ExceptionHandlingMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (OperationCanceledException) when (context.RequestAborted.IsCancellationRequested)
        {
            // Cliente desistiu: não é erro do servidor
            context.Response.StatusCode = 499;
        }
        catch (ValidationException ex)
        {
            logger.LogWarning(
                "Validação falhou em {Method} {Path}: {Errors}",
                context.Request.Method,
                context.Request.Path,
                string.Join("; ", ex.Errors.Select(e => e.ErrorMessage)));

            await WriteProblem(
                context,
                StatusCodes.Status400BadRequest,
                "Falha de validação",
                ex.Errors
                    .GroupBy(e => e.PropertyName)
                    .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray()));
        }
        catch (Exception ex)
        {
            logger.LogError(
                ex,
                "Exceção não tratada em {Method} {Path}",
                context.Request.Method,
                context.Request.Path);

            await WriteProblem(context, StatusCodes.Status500InternalServerError, "Ocorreu um erro inesperado.");
        }
    }

    private static Task WriteProblem(
        HttpContext context,
        int status,
        string title,
        IDictionary<string, string[]>? errors = null)
    {
        if (context.Response.HasStarted)
        {
            return Task.CompletedTask;
        }

        context.Response.Clear();
        context.Response.StatusCode = status;

        ProblemDetails problem = errors is null
            ? new ProblemDetails { Status = status, Title = title, Instance = context.Request.Path }
            : new ValidationProblemDetails(errors) { Status = status, Title = title, Instance = context.Request.Path };

        problem.Extensions["traceId"] = context.TraceIdentifier;
        return context.Response.WriteAsJsonAsync(problem, options: null, contentType: "application/problem+json");
    }
}

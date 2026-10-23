using Common.Results;
using FluentResults;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;

namespace Api.Results;

public static class ResultExtensions
{
    /// <summary>200 com o valor ou ProblemDetails conforme o tipo do primeiro erro.</summary>
    public static IResult ToIResult<T>(this Result<T> result) =>
        result.IsSuccess ? (IResult)TypedResults.Ok(result.Value) : ToProblem(result.Errors);

    /// <summary>204 ou ProblemDetails conforme o tipo do primeiro erro.</summary>
    public static IResult ToIResult(this Result result) =>
        result.IsSuccess ? (IResult)TypedResults.NoContent() : ToProblem(result.Errors);

    /// <summary>422 com ValidationProblemDetails (RFC 7807).</summary>
    public static IResult ToValidationResult(this ValidationResult validationResult)
    {
        var errors = validationResult.Errors
            .GroupBy(e => e.PropertyName)
            .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());

        return new ProblemResult(
            StatusCodes.Status422UnprocessableEntity,
            "Falha de validação",
            null,
            new Dictionary<string, object?>(),
            errors);
    }

    private static IResult ToProblem(IReadOnlyList<IError> errors)
    {
        var error = errors.FirstOrDefault();
        var type = error is not null && error.Metadata.TryGetValue(Errors.TypeKey, out var t)
            ? t?.ToString()
            : null;

        var (status, title) = type switch
        {
            "Validation" => (StatusCodes.Status422UnprocessableEntity, "Falha de validação"),
            "NotFound" => (StatusCodes.Status404NotFound, "Recurso não encontrado"),
            "Conflict" => (StatusCodes.Status409Conflict, "Conflito"),
            "CapacidadeIndisponivel" => (StatusCodes.Status409Conflict, "Capacidade indisponível"),
            "Pagamento" => (StatusCodes.Status402PaymentRequired, "Falha no pagamento"),
            "Unauthorized" => (StatusCodes.Status401Unauthorized, "Não autenticado"),
            "Forbidden" => (StatusCodes.Status403Forbidden, "Acesso negado"),
            _ => (StatusCodes.Status500InternalServerError, "Erro interno"),
        };

        // 500 nunca vaza a mensagem original
        var detail = status == StatusCodes.Status500InternalServerError
            ? "Ocorreu um erro inesperado."
            : error?.Message;

        var extensions = new Dictionary<string, object?>();

        if (type == "CapacidadeIndisponivel"
            && error is not null
            && error.Metadata.TryGetValue("Sugestoes", out var sugestoes))
        {
            extensions["sugestoes"] = sugestoes;
        }

        return new ProblemResult(status, title, detail, extensions);
    }

    /// <summary>Resposta ProblemDetails que injeta o traceId da requisição.</summary>
    private sealed class ProblemResult(
        int status,
        string title,
        string? detail,
        Dictionary<string, object?> extensions,
        IDictionary<string, string[]>? validationErrors = null) : IResult
    {
        public Task ExecuteAsync(HttpContext httpContext)
        {
            ProblemDetails problem = validationErrors is null
                ? new ProblemDetails()
                : new ValidationProblemDetails(validationErrors);

            problem.Status = status;
            problem.Title = title;
            problem.Detail = detail;
            problem.Instance = httpContext.Request.Path;

            foreach (var (key, value) in extensions)
            {
                problem.Extensions[key] = value;
            }

            problem.Extensions["traceId"] = httpContext.TraceIdentifier;

            httpContext.Response.StatusCode = status;
            return httpContext.Response.WriteAsJsonAsync(problem, options: null, contentType: "application/problem+json");
        }
    }
}

using FluentResults;

namespace Common.Results;

/// <summary>Fábrica de erros padronizados. Cada erro carrega a metadata "Type", usada pela API para mapear o status HTTP.</summary>
public static class Errors
{
    public const string TypeKey = "Type";

    /// <summary>Erro de validação (422).</summary>
    public static IError Validation(string message)
        => new Error(message).WithMetadata(TypeKey, "Validation");

    /// <summary>Recurso não encontrado (404).</summary>
    public static IError NotFound(string message)
        => new Error(message).WithMetadata(TypeKey, "NotFound");

    /// <summary>Conflito de estado (409).</summary>
    public static IError Conflict(string message)
        => new Error(message).WithMetadata(TypeKey, "Conflict");

    /// <summary>Capacidade de produção indisponível (409), com sugestões de alternativas.</summary>
    public static IError CapacidadeIndisponivel(string message, IReadOnlyList<string> sugestoes)
        => new Error(message)
            .WithMetadata(TypeKey, "CapacidadeIndisponivel")
            .WithMetadata("Sugestoes", sugestoes);

    /// <summary>Falha de pagamento (402).</summary>
    public static IError Pagamento(string message)
        => new Error(message).WithMetadata(TypeKey, "Pagamento");

    /// <summary>Não autenticado (401).</summary>
    public static IError Unauthorized(string message)
        => new Error(message).WithMetadata(TypeKey, "Unauthorized");

    /// <summary>Sem permissão (403).</summary>
    public static IError Forbidden(string message)
        => new Error(message).WithMetadata(TypeKey, "Forbidden");

    /// <summary>Converte erros agrupados por campo em um único erro de validação.</summary>
    public static IError ValidationFromDictionary(IDictionary<string, string[]> errors)
    {
        var message = string.Join("; ", errors.SelectMany(e => e.Value));
        return new Error(message)
            .WithMetadata(TypeKey, "Validation")
            .WithMetadata("Errors", errors);
    }
}

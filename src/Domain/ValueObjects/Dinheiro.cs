using FluentResults;
using Common.Results;

namespace Domain.ValueObjects;

/// <summary>Valor monetário em reais, não negativo e com no máximo 2 casas decimais.</summary>
public readonly record struct Dinheiro(decimal Valor)
{
    public static Dinheiro Zero => new(0m);

    public static Result<Dinheiro> Create(decimal valor)
    {
        if (valor < 0)
        {
            return Result.Fail(Errors.Validation("O valor monetário não pode ser negativo."));
        }

        if (decimal.Round(valor, 2) != valor)
        {
            return Result.Fail(Errors.Validation("O valor monetário deve ter no máximo 2 casas decimais."));
        }

        return Result.Ok(new Dinheiro(valor));
    }

    public static Dinheiro operator +(Dinheiro a, Dinheiro b) => new(a.Valor + b.Valor);

    public static Dinheiro operator -(Dinheiro a, Dinheiro b) => new(a.Valor - b.Valor);

    public static Dinheiro operator *(Dinheiro a, decimal fator) => new(decimal.Round(a.Valor * fator, 2));

    public static Dinheiro operator *(decimal fator, Dinheiro a) => a * fator;

    public override string ToString() => Valor.ToString("C2", System.Globalization.CultureInfo.GetCultureInfo("pt-BR"));
}

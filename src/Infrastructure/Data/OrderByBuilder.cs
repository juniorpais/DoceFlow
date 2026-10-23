namespace Infrastructure.Data;

public static class OrderByBuilder
{
    /// <summary>
    /// Converte "data desc, numeroVenda asc" em "DataVenda DESC, NumeroVenda ASC",
    /// aceitando apenas campos presentes em <paramref name="allowed"/>.
    /// Campos desconhecidos são ignorados; sem nenhuma cláusula válida usa-se <paramref name="fallback"/>.
    /// </summary>
    public static string Build(
        string? order,
        IReadOnlyDictionary<string, string> allowed,
        string fallback)
    {
        if (string.IsNullOrWhiteSpace(order))
        {
            return fallback;
        }

        var clauses = order
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Select(c => c.Split(' ', StringSplitOptions.RemoveEmptyEntries))
            .Where(p => p.Length > 0 && allowed.ContainsKey(p[0]))
            .Select(p =>
            {
                var desc = p.Length > 1 && p[1].Equals("desc", StringComparison.OrdinalIgnoreCase);
                return $"{allowed[p[0]]} {(desc ? "DESC" : "ASC")}";
            })
            .ToList();

        return clauses.Count > 0 ? string.Join(", ", clauses) : fallback;
    }
}

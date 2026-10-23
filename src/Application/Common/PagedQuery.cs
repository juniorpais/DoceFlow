namespace Application.Common;

public abstract record PagedQuery
{
    public int Page { get; init; } = 1;

    public int Size { get; init; } = 10;

    /// <summary>Ex.: "data desc, numeroVenda asc"</summary>
    public string? Order { get; init; }
}

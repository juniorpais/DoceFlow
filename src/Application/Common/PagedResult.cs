namespace Application.Common;

public sealed record PagedResult<T>(
    IReadOnlyList<T> Data,
    int TotalItems,
    int CurrentPage,
    int TotalPages)
{
    public static PagedResult<T> Create(IReadOnlyList<T> data, int total, int page, int size)
        => new(data, total, page, (int)Math.Ceiling(total / (double)size));
}

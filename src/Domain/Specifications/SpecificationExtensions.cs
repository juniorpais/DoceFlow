namespace Domain.Specifications;

/// <summary>Combinadores de especificações.</summary>
public static class SpecificationExtensions
{
    public static ISpecification<T> And<T>(this ISpecification<T> left, ISpecification<T> right)
        => new DelegateSpecification<T>(e => left.IsSatisfiedBy(e) && right.IsSatisfiedBy(e));

    public static ISpecification<T> Or<T>(this ISpecification<T> left, ISpecification<T> right)
        => new DelegateSpecification<T>(e => left.IsSatisfiedBy(e) || right.IsSatisfiedBy(e));

    public static ISpecification<T> Not<T>(this ISpecification<T> specification)
        => new DelegateSpecification<T>(e => !specification.IsSatisfiedBy(e));

    private sealed class DelegateSpecification<T>(Func<T, bool> predicate) : ISpecification<T>
    {
        public bool IsSatisfiedBy(T entity) => predicate(entity);
    }
}

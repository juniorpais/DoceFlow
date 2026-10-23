using Domain.Specifications;
using FluentAssertions;

namespace Unit.Domain;

public sealed class SpecificationTests
{
    private sealed class PredicateSpec(Func<int, bool> predicate) : ISpecification<int>
    {
        public bool IsSatisfiedBy(int entity) => predicate(entity);
    }

    private static readonly ISpecification<int> Positivo = new PredicateSpec(n => n > 0);
    private static readonly ISpecification<int> Par = new PredicateSpec(n => n % 2 == 0);

    [Theory(DisplayName = "Given two specs When combining with And Then both must be satisfied")]
    [InlineData(4, true)]
    [InlineData(3, false)]
    [InlineData(-4, false)]
    public void Given_TwoSpecs_When_CombiningWithAnd_Then_BothMustBeSatisfied(int value, bool expected)
        => Positivo.And(Par).IsSatisfiedBy(value).Should().Be(expected);

    [Theory(DisplayName = "Given two specs When combining with Or Then either may be satisfied")]
    [InlineData(3, true)]
    [InlineData(-4, true)]
    [InlineData(-3, false)]
    public void Given_TwoSpecs_When_CombiningWithOr_Then_EitherMayBeSatisfied(int value, bool expected)
        => Positivo.Or(Par).IsSatisfiedBy(value).Should().Be(expected);

    [Theory(DisplayName = "Given a spec When negating Then inverts the result")]
    [InlineData(3, false)]
    [InlineData(-3, true)]
    public void Given_Spec_When_Negating_Then_InvertsResult(int value, bool expected)
        => Positivo.Not().IsSatisfiedBy(value).Should().Be(expected);
}

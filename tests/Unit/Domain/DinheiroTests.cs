using Domain.ValueObjects;
using FluentAssertions;

namespace Unit.Domain;

public sealed class DinheiroTests
{
    [Fact(DisplayName = "Given a negative value When creating Dinheiro Then fails")]
    public void Given_NegativeValue_When_CreatingDinheiro_Then_Fails()
    {
        var result = Dinheiro.Create(-0.01m);

        result.IsFailed.Should().BeTrue();
    }

    [Theory(DisplayName = "Given a non-negative value When creating Dinheiro Then succeeds")]
    [InlineData(0)]
    [InlineData(10.5)]
    [InlineData(999.99)]
    public void Given_NonNegativeValue_When_CreatingDinheiro_Then_Succeeds(double valor)
    {
        var result = Dinheiro.Create((decimal)valor);

        result.IsSuccess.Should().BeTrue();
        result.Value.Valor.Should().Be((decimal)valor);
    }

    [Fact(DisplayName = "Given a value with three decimals When creating Dinheiro Then fails")]
    public void Given_ThreeDecimals_When_CreatingDinheiro_Then_Fails()
    {
        var result = Dinheiro.Create(10.456m);

        result.IsFailed.Should().BeTrue();
    }

    [Fact(DisplayName = "Given an amount When multiplying by a fractional factor Then rounds to two decimals")]
    public void Given_Amount_When_MultiplyingByFractionalFactor_Then_RoundsToTwoDecimals()
    {
        var product = new Dinheiro(10m) * 0.3333m;

        product.Valor.Should().Be(3.33m);
    }

    [Fact(DisplayName = "Given two amounts When adding Then returns the sum")]
    public void Given_TwoAmounts_When_Adding_Then_ReturnsSum()
    {
        var sum = new Dinheiro(10.25m) + new Dinheiro(4.75m);

        sum.Valor.Should().Be(15m);
    }

    [Fact(DisplayName = "Given two amounts When subtracting Then returns the difference")]
    public void Given_TwoAmounts_When_Subtracting_Then_ReturnsDifference()
    {
        var difference = new Dinheiro(10m) - new Dinheiro(3.5m);

        difference.Valor.Should().Be(6.5m);
    }

    [Fact(DisplayName = "Given an amount When multiplying by a quantity Then returns the product")]
    public void Given_Amount_When_MultiplyingByQuantity_Then_ReturnsProduct()
    {
        var product = new Dinheiro(12.5m) * 3;

        product.Valor.Should().Be(37.5m);
    }

    [Fact(DisplayName = "Given Zero When reading Valor Then is zero")]
    public void Given_Zero_When_ReadingValor_Then_IsZero()
    {
        Dinheiro.Zero.Valor.Should().Be(0m);
    }
}

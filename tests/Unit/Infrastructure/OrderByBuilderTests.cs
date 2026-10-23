using FluentAssertions;
using Infrastructure.Data;

namespace Unit.Infrastructure;

public sealed class OrderByBuilderTests
{
    private const string Fallback = "DataVenda DESC";

    private static readonly IReadOnlyDictionary<string, string> Allowed =
        new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            ["data"] = "DataVenda",
            ["numeroVenda"] = "NumeroVenda",
        };

    [Fact(DisplayName = "Given known fields with directions When building Then maps columns and directions")]
    public void Given_KnownFieldsWithDirections_When_Building_Then_MapsColumnsAndDirections()
    {
        var result = OrderByBuilder.Build("data desc, numeroVenda asc", Allowed, Fallback);

        result.Should().Be("DataVenda DESC, NumeroVenda ASC");
    }

    [Fact(DisplayName = "Given an unknown field among known ones When building Then ignores the unknown field")]
    public void Given_UnknownFieldAmongKnown_When_Building_Then_IgnoresUnknown()
    {
        var result = OrderByBuilder.Build("hack; DROP TABLE x desc, numeroVenda asc", Allowed, Fallback);

        result.Should().Be("NumeroVenda ASC");
    }

    [Theory(DisplayName = "Given an empty order When building Then returns the fallback")]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Given_EmptyOrder_When_Building_Then_ReturnsFallback(string? order)
    {
        OrderByBuilder.Build(order, Allowed, Fallback).Should().Be(Fallback);
    }

    [Fact(DisplayName = "Given only unknown fields When building Then returns the fallback")]
    public void Given_OnlyUnknownFields_When_Building_Then_ReturnsFallback()
    {
        OrderByBuilder.Build("foo desc, bar asc", Allowed, Fallback).Should().Be(Fallback);
    }
}

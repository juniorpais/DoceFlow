using Application.Common;
using FluentAssertions;

namespace Unit.Application;

public sealed class PagedResultTests
{
    public sealed record ListarQuery : PagedQuery;

    public sealed class ListarQueryValidator : PagedQueryValidator<ListarQuery>;

    [Theory(DisplayName = "Given total items and size When creating PagedResult Then TotalPages rounds up")]
    [InlineData(0, 10, 0)]
    [InlineData(1, 10, 1)]
    [InlineData(10, 10, 1)]
    [InlineData(11, 10, 2)]
    [InlineData(95, 10, 10)]
    [InlineData(101, 100, 2)]
    public void Given_TotalItemsAndSize_When_CreatingPagedResult_Then_TotalPagesRoundsUp(int total, int size, int expectedPages)
    {
        var result = PagedResult<string>.Create([], total, 1, size);

        result.TotalItems.Should().Be(total);
        result.TotalPages.Should().Be(expectedPages);
    }

    [Fact(DisplayName = "Given data and page When creating PagedResult Then keeps data and current page")]
    public void Given_DataAndPage_When_CreatingPagedResult_Then_KeepsDataAndCurrentPage()
    {
        var result = PagedResult<string>.Create(["a", "b"], 12, 2, 5);

        result.Data.Should().Equal("a", "b");
        result.CurrentPage.Should().Be(2);
        result.TotalPages.Should().Be(3);
    }

    [Fact(DisplayName = "Given default paging When validating Then is valid")]
    public void Given_DefaultPaging_When_Validating_Then_IsValid()
    {
        var result = new ListarQueryValidator().Validate(new ListarQuery());

        result.IsValid.Should().BeTrue();
    }

    [Fact(DisplayName = "Given page zero When validating Then fails on Page")]
    public void Given_PageZero_When_Validating_Then_FailsOnPage()
    {
        var result = new ListarQueryValidator().Validate(new ListarQuery { Page = 0 });

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(PagedQuery.Page));
    }

    [Fact(DisplayName = "Given size 101 When validating Then fails on Size")]
    public void Given_Size101_When_Validating_Then_FailsOnSize()
    {
        var result = new ListarQueryValidator().Validate(new ListarQuery { Size = 101 });

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(PagedQuery.Size));
    }

    [Fact(DisplayName = "Given size 100 When validating Then is valid")]
    public void Given_Size100_When_Validating_Then_IsValid()
    {
        var result = new ListarQueryValidator().Validate(new ListarQuery { Size = 100 });

        result.IsValid.Should().BeTrue();
    }
}

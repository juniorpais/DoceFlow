using Common.Results;
using FluentAssertions;

namespace Unit.Common;

public sealed class ErrorsTests
{
    public static TheoryData<string, Func<FluentResults.IError>> Factories => new()
    {
        { "Validation", () => Errors.Validation("msg") },
        { "NotFound", () => Errors.NotFound("msg") },
        { "Conflict", () => Errors.Conflict("msg") },
        { "CapacidadeIndisponivel", () => Errors.CapacidadeIndisponivel("msg", ["a"]) },
        { "Pagamento", () => Errors.Pagamento("msg") },
        { "Unauthorized", () => Errors.Unauthorized("msg") },
        { "Forbidden", () => Errors.Forbidden("msg") },
    };

    [Theory(DisplayName = "Given an error factory When creating the error Then sets the expected Type metadata and message")]
    [MemberData(nameof(Factories))]
    public void Given_ErrorFactory_When_CreatingError_Then_SetsExpectedTypeMetadata(string expectedType, Func<FluentResults.IError> factory)
    {
        var error = factory();

        error.Message.Should().Be("msg");
        error.Metadata.Should().ContainKey(Errors.TypeKey);
        error.Metadata[Errors.TypeKey].Should().Be(expectedType);
    }

    [Fact(DisplayName = "Given suggestions When creating CapacidadeIndisponivel Then carries Sugestoes metadata")]
    public void Given_Suggestions_When_CreatingCapacidadeIndisponivel_Then_CarriesSugestoes()
    {
        IReadOnlyList<string> sugestoes = ["2026-10-25", "2026-10-26"];

        var error = Errors.CapacidadeIndisponivel("Sem capacidade", sugestoes);

        error.Metadata.Should().ContainKey("Sugestoes");
        ((IEnumerable<string>)error.Metadata["Sugestoes"]).Should().Equal(sugestoes);
    }

    [Fact(DisplayName = "Given a dictionary of errors When creating ValidationFromDictionary Then returns a Validation error")]
    public void Given_ErrorDictionary_When_CreatingValidationFromDictionary_Then_ReturnsValidationError()
    {
        var errors = new Dictionary<string, string[]> { ["Nome"] = ["Obrigatório"] };

        var error = Errors.ValidationFromDictionary(errors);

        error.Metadata[Errors.TypeKey].Should().Be("Validation");
    }
}

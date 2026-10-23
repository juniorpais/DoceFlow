using Application.Abstractions;
using Application.Features.Sistema;
using FluentAssertions;
using NSubstitute;

namespace Unit.Application;

public sealed class PingHandlerTests
{
    private readonly IDateTimeProvider _clock = Substitute.For<IDateTimeProvider>();
    private readonly PingHandler _handler;

    public PingHandlerTests() => _handler = new PingHandler(_clock);

    [Fact(DisplayName = "Given a fixed clock When handling ping Then returns pong with the provider date")]
    public async Task Given_FixedClock_When_HandlingPing_Then_ReturnsPongWithProviderDate()
    {
        var now = new DateTime(2026, 10, 23, 12, 30, 0, DateTimeKind.Utc);
        _clock.UtcNow.Returns(now);

        var result = await _handler.HandleAsync(new PingQuery(), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Mensagem.Should().Be("pong");
        result.Value.ServidorUtc.Should().Be(now);
    }
}

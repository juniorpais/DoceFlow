using System.Net;
using System.Net.Http.Json;
using Application.Features.Sistema;
using FluentAssertions;
using Functional.Common;

namespace Functional.Sistema;

public sealed class SmokeTests(FunctionalWebApplicationFactory factory) : IClassFixture<FunctionalWebApplicationFactory>
{
    private readonly HttpClient _client = factory.CreateClient();

    [Fact(DisplayName = "Given running app When pinging Then returns pong with a recent UTC timestamp")]
    public async Task Given_RunningApp_When_Pinging_Then_ReturnsPongWithRecentTimestamp()
    {
        var before = DateTime.UtcNow.AddMinutes(-1);

        var response = await _client.GetAsync("/api/v1/sistema/ping");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<PingResponse>();
        body.Should().NotBeNull();
        body!.Mensagem.Should().Be("pong");
        body.ServidorUtc.ToUniversalTime().Should().BeAfter(before);
    }
}

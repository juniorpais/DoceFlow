using System.Net;
using System.Net.Http.Json;
using Application.Features.Sistema;
using FluentAssertions;
using Integration.Common;

namespace Integration.Sistema;

public sealed class SistemaEndpointsTests(CustomWebApplicationFactory factory) : BaseIntegrationTest(factory)
{
    [Fact(DisplayName = "Given running app When requesting ping Then returns 200 with pong message")]
    public async Task Given_RunningApp_When_RequestingPing_Then_Returns200WithPong()
    {
        var response = await Client.GetAsync("/api/v1/sistema/ping");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<PingResponse>();
        body.Should().NotBeNull();
        body!.Mensagem.Should().Be("pong");
    }
}

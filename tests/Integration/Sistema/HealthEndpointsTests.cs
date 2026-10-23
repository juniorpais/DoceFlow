using System.Net;
using System.Text.Json;
using FluentAssertions;
using Integration.Common;

namespace Integration.Sistema;

public sealed class HealthEndpointsTests(CustomWebApplicationFactory factory) : BaseIntegrationTest(factory)
{
    [Fact(DisplayName = "Given running app When requesting health live Then returns 200 with Healthy status")]
    public async Task Given_RunningApp_When_RequestingHealthLive_Then_Returns200Healthy()
    {
        var response = await Client.GetAsync("/health/live");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        using var json = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        json.RootElement.GetProperty("status").GetString().Should().Be("Healthy");
    }
}

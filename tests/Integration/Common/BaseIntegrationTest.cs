namespace Integration.Common;

public abstract class BaseIntegrationTest : IClassFixture<CustomWebApplicationFactory>
{
    protected BaseIntegrationTest(CustomWebApplicationFactory factory) => Client = factory.CreateClient();

    protected HttpClient Client { get; }
}

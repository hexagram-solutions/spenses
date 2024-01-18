using FluentAssertions.Extensions;

namespace Spenses.Api.IntegrationTests;

public class WebApplicationFixture<TStartup> : IAsyncLifetime
    where TStartup : class
{
    public WebApplicationFixture()
    {
        WebApplicationFactory = new TestWebApplicationFactory<TStartup>();

        // Since this is static, it will set equivalency rules for the entire test run. This could be configured
        // anywhere, but this class is "global" enough for such configuration.
        AssertionOptions.AssertEquivalencyUsing(opts =>
            opts.Using<DateTime>(ctx =>
                ctx.Subject.Should().BeCloseTo(ctx.Expectation, 100.Milliseconds())).WhenTypeIs<DateTime>());
    }

    public virtual TestWebApplicationFactory<TStartup> WebApplicationFactory { get; }

    public Task InitializeAsync()
    {
        // Add any initialization logic to set up integration tests here.
        return Task.CompletedTask;
    }

    public async Task DisposeAsync()
    {
        await WebApplicationFactory.DisposeAsync();
    }
}

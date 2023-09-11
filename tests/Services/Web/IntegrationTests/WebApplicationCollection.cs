using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions.Extensions;

namespace Spenses.Application.IntegrationTests;

[CollectionDefinition(CollectionName, DisableParallelization = true)]
public class WebApplicationCollection : ICollectionFixture<WebApplicationFixture<Program>>
{
    public const string CollectionName = "Web Application";

    // This class has no code, and is never created. Its purpose is simply to be the place to apply
    // [CollectionDefinition] and all the ICollectionFixture<> interfaces.
}

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

    public TestWebApplicationFactory<TStartup> WebApplicationFactory { get; }

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

using FluentAssertions.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Spenses.Resources.Relational;
using Spenses.Resources.Relational.Infrastructure;

namespace Spenses.Api.IntegrationTests;

public class IdentityWebApplicationFixture : IAsyncLifetime
{
    public IdentityWebApplicationFixture()
    {
        WebApplicationFactory = new IdentityWebApplicationFactory();

        // Since this is static, it will set equivalency rules for the entire test run. This could be configured
        // anywhere, but this class is "global" enough for such configuration.
        AssertionOptions.AssertEquivalencyUsing(opts =>
            opts.Using<DateTime>(ctx =>
                ctx.Subject.Should().BeCloseTo(ctx.Expectation, 100.Milliseconds())).WhenTypeIs<DateTime>());
    }

    public IdentityWebApplicationFactory WebApplicationFactory { get; }

    public async Task InitializeAsync()
    {
        await WebApplicationFactory.InitializeAsync();

        var db = WebApplicationFactory.Services.GetRequiredService<ApplicationDbContext>();

        await db.Database.MigrateAsync();
        await db.UpdateViews();
    }

    public async Task DisposeAsync()
    {
        await WebApplicationFactory.DisposeAsync();
    }
}

using FluentAssertions.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Refit;
using Spenses.Api.IntegrationTests.Identity.Services;
using Spenses.Client.Http;
using Spenses.Resources.Relational.Models;
using Spenses.Shared.Models.Identity;

namespace Spenses.Api.IntegrationTests.Identity;

public class IdentityWebApplicationFixture<TStartup> : IAsyncLifetime
    where TStartup : class
{
    public IdentityWebApplicationFixture()
    {
        WebApplicationFactory = new IdentityWebApplicationFactory<TStartup>();

        // Since this is static, it will set equivalency rules for the entire test run. This could be configured
        // anywhere, but this class is "global" enough for such configuration.
        AssertionOptions.AssertEquivalencyUsing(opts =>
            opts.Using<DateTime>(ctx =>
                ctx.Subject.Should().BeCloseTo(ctx.Expectation, 100.Milliseconds())).WhenTypeIs<DateTime>());
    }

    public IdentityWebApplicationFactory<TStartup> WebApplicationFactory { get; }

    public CurrentUser RegisteredUser { get; private set; } = null!;

    public async Task InitializeAsync()
    {
        var identityApi = RestService.For<IIdentityApi>(WebApplicationFactory.CreateClient());

        var response = await identityApi.Register(new RegisterRequest
        {
            Email = "grunky.peep@georgiasouthern.edu",
            Password = "Password123!",
            Name = "Grunky Peep"
        });

        RegisteredUser = response.Content!;
    }

    public async Task DisposeAsync()
    {
        await DeleteUser("grunky.peep@georgiasouthern.edu");

        var emailClient = WebApplicationFactory.Services.GetRequiredService<CapturingEmailClient>();
        emailClient.EmailMessages.Clear();

        await WebApplicationFactory.DisposeAsync();
    }

    public async Task DeleteUser(string email)
    {
        var userManager = WebApplicationFactory.Services.GetRequiredService<UserManager<ApplicationUser>>();

        var user = await userManager.FindByEmailAsync(email);

        await userManager.DeleteAsync(user!);
    }
}

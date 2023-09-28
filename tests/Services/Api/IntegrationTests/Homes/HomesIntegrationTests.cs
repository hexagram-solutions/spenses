using System.Net;
using FluentAssertions.Execution;
using Microsoft.Extensions.DependencyInjection;
using Refit;
using Spenses.Application.Models;
using Spenses.Client.Http;
using Spenses.Resources.Relational;
using DbModels = Spenses.Resources.Relational.Models;

namespace Spenses.Api.IntegrationTests.Homes;

[Collection(WebApplicationCollection.CollectionName)]
public class HomesIntegrationTests
{
    private readonly WebApplicationFixture<Program> _fixture;
    private readonly IHomesApi _homes;

    public HomesIntegrationTests(WebApplicationFixture<Program> fixture)
    {
        _fixture = fixture;
        _homes = RestService.For<IHomesApi>(fixture.WebApplicationFactory.CreateClient());
    }

    [Fact]
    public async Task Post_creates_home()
    {
        var properties = new HomeProperties { Name = "foo", Description = "bar" };

        var createdHome = (await _homes.PostHome(properties)).Content!;
        createdHome.Should().BeEquivalentTo(properties, opts =>
            opts.ExcludingNestedObjects()
                .ExcludingMissingMembers());

        var retrievedHome = (await _homes.GetHome(createdHome.Id)).Content;
        retrievedHome.Should().BeEquivalentTo(createdHome);

        var homes = await _homes.GetHomes();
        homes.Content.Should().ContainEquivalentOf(retrievedHome);

        await _homes.DeleteHome(createdHome.Id);
    }

    [Fact]
    public async Task Put_home_updates_home()
    {
        var home = (await _homes.GetHomes()).Content!.First();

        var properties = new HomeProperties { Name = "sut", Description = "baz" };

        var updatedHome = (await _homes.PutHome(home.Id, properties)).Content!;

        updatedHome.Should().BeEquivalentTo(properties, opts =>
            opts.ExcludingNestedObjects()
                .ExcludingMissingMembers());

        var fetchedHome = await _homes.GetHome(updatedHome.Id);
        fetchedHome.Content.Should().BeEquivalentTo(updatedHome);
    }

    [Fact]
    public async Task Get_home_where_current_user_is_not_a_member_returns_not_found()
    {
        async Task<Guid> SetUp()
        {
            await using var scope = _fixture.WebApplicationFactory.Services.CreateAsyncScope();

            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            var homeEntry = await db.Homes.AddAsync(new DbModels.Home { Name = "foo" });

            await db.SaveChangesAsync();

            return homeEntry.Entity.Id;
        }

        using (new AssertionScope())
        {
            var homeId = await SetUp();

            var homeResponse = await _homes.GetHome(homeId);

            homeResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);

            await TearDown(homeId);
        }

        async Task TearDown(Guid homeId)
        {
            await using var scope = _fixture.WebApplicationFactory.Services.CreateAsyncScope();

            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            var home = await db.Homes.FindAsync(homeId);

            db.Homes.Remove(home!);

            await db.SaveChangesAsync();
        }
    }
}

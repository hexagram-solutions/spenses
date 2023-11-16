using System.Net;
using FluentAssertions.Execution;
using Microsoft.Extensions.DependencyInjection;
using Spenses.Resources.Relational;
using DbModels = Spenses.Resources.Relational.Models;

namespace Spenses.Api.IntegrationTests.Homes;

public partial class HomesIntegrationTests
{
    [Fact]
    public async Task Get_non_existent_home_yields_not_found_result()
    {
        var result = await _homes.GetHome(Guid.NewGuid());

        result.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Get_home_where_current_user_is_not_a_member_returns_unauthorized()
    {
        async Task<Guid> SetUp()
        {
            await using var scope = fixture.WebApplicationFactory.Services.CreateAsyncScope();

            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            var homeEntry = await db.Homes.AddAsync(new DbModels.Home { Name = "foo" });

            await db.SaveChangesAsync();

            return homeEntry.Entity.Id;
        }

        using (new AssertionScope())
        {
            var homeId = await SetUp();

            var homeResponse = await _homes.GetHome(homeId);

            homeResponse.Error!.StatusCode.Should().Be(HttpStatusCode.Forbidden);

            await TearDown(homeId);
        }

        async Task TearDown(Guid homeId)
        {
            await using var scope = fixture.WebApplicationFactory.Services.CreateAsyncScope();

            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            var home = await db.Homes.FindAsync(homeId);

            db.Homes.Remove(home!);

            await db.SaveChangesAsync();
        }
    }
}

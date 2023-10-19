using Spenses.Application.Models.Homes;

namespace Spenses.Api.IntegrationTests.Homes;

public partial class HomesIntegrationTests
{
    [Fact]
    public async Task Put_home_updates_home()
    {
        var home = (await _homes.GetHomes()).Content!.First();

        var properties = new HomeProperties
        {
            Name = "sut",
            Description = "baz",
            ExpensePeriod = ExpensePeriod.Weekly
        };

        var updatedHome = (await _homes.PutHome(home.Id, properties)).Content!;

        updatedHome.Should().BeEquivalentTo(properties, opts =>
            opts.ExcludingNestedObjects()
                .ExcludingMissingMembers());

        var fetchedHome = await _homes.GetHome(updatedHome.Id);
        fetchedHome.Content.Should().BeEquivalentTo(updatedHome);
    }
}

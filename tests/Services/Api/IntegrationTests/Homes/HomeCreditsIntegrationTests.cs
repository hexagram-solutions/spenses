using System.Net;
using Refit;
using Spenses.Application.Models;
using Spenses.Client.Http;

namespace Spenses.Api.IntegrationTests.Homes;

[Collection(WebApplicationCollection.CollectionName)]
public class HomeCreditsIntegrationTests
{
    private readonly IHomesApi _homes;
    private readonly IHomeCreditsApi _homeCredits;

    public HomeCreditsIntegrationTests(WebApplicationFixture<Program> fixture)
    {
        _homes = RestService.For<IHomesApi>(fixture.WebApplicationFactory.CreateClient());
        _homeCredits = RestService.For<IHomeCreditsApi>(fixture.WebApplicationFactory.CreateClient());
    }

    [Fact]
    public async Task Post_credit_creates_credit()
    {
        var home = (await _homes.GetHomes()).Content!.First();

        var properties = new CreditProperties
        {
            Amount = 1234.56m,
            Date = DateOnly.FromDateTime(DateTime.UtcNow),
            PaidByMemberId = home.Members.First().Id
        };

        var createdCreditResponse = await _homeCredits.PostHomeCredit(home.Id, properties);

        createdCreditResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        var createdCredit = createdCreditResponse.Content;

        createdCredit.Should().BeEquivalentTo(properties, opts =>
            opts.ExcludingNestedObjects()
                .ExcludingMissingMembers());

        var fetchedCredit = (await _homeCredits.GetHomeCredit(home.Id, createdCredit!.Id)).Content;
        fetchedCredit.Should().BeEquivalentTo(createdCredit);

        var credits = (await _homeCredits.GetHomeCredits(home.Id)).Content;
        credits.Should().ContainEquivalentOf(createdCredit);

        await _homeCredits.DeleteHomeCredit(home.Id, createdCredit.Id);
    }

    [Fact]
    public async Task Put_credit_creates_credit()
    {
        var home = (await _homes.GetHomes()).Content!.First();

        var credit = (await _homeCredits.GetHomeCredits(home.Id)).Content!.First();

        var properties = new CreditProperties
        {
            Amount = 1234.56m,
            Date = DateOnly.FromDateTime(DateTime.UtcNow),
            PaidByMemberId = home.Members.First().Id
        };

        var updatedCreditResponse = await _homeCredits.PutHomeCredit(home.Id, credit.Id, properties);

        updatedCreditResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var updatedCredit = updatedCreditResponse.Content;

        updatedCredit.Should().BeEquivalentTo(properties, opts =>
            opts.ExcludingNestedObjects()
                .ExcludingMissingMembers());

        var fetchedCredit = (await _homeCredits.GetHomeCredit(home.Id, updatedCredit!.Id)).Content;

        fetchedCredit.Should().BeEquivalentTo(updatedCredit);
    }
}

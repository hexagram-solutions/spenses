using System.Net;
using Refit;
using Spenses.Application.Models.Common;
using Spenses.Application.Models.Credits;
using Spenses.Client.Http;

namespace Spenses.Api.IntegrationTests.Controllers;

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
            Note = "foobar",
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

        await _homeCredits.DeleteHomeCredit(home.Id, createdCredit.Id);
    }

    [Fact]
    public async Task Post_invalid_credit_yields_bad_request()
    {
        var home = (await _homes.GetHomes()).Content!.First();

        var result = await _homeCredits.PostHomeCredit(home.Id, new CreditProperties());

        result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Put_credit_creates_credit()
    {
        var home = (await _homes.GetHomes()).Content!.First();

        var credit = (await _homeCredits.GetHomeCredits(home.Id, new FilteredCreditsQuery
        {
            PageNumber = 1,
            PageSize = 100
        })).Content!.Items.First();

        var properties = new CreditProperties
        {
            Amount = 1234.56m,
            Date = DateOnly.FromDateTime(DateTime.UtcNow),
            Note = "foobar",
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

    [Fact]
    public async Task Get_credits_with_period_filters_yields_Credits_in_range()
    {
        var home = (await _homes.GetHomes()).Content!.First();

        var unfilteredCredits = (await _homeCredits.GetHomeCredits(home.Id, new FilteredCreditsQuery
        {
            PageNumber = 1,
            PageSize = 100
        })).Content!.Items.ToList();

        var earliestCreditDate = unfilteredCredits.MinBy(x => x.Date)!.Date;
        var latestCreditDate = unfilteredCredits.MaxBy(x => x.Date)!.Date;

        var minDateFilterValue = earliestCreditDate.AddDays(1);
        var maxDateFilterValue = latestCreditDate.AddDays(-1);

        var filteredCredits = (await _homeCredits.GetHomeCredits(home.Id, new FilteredCreditsQuery
        {
            PageNumber = 1,
            PageSize = 100,
            MinDate = minDateFilterValue,
            MaxDate = maxDateFilterValue
        })).Content!.Items;

        filteredCredits.Should().AllSatisfy(e =>
        {
            e.Date.Should().BeOnOrAfter(minDateFilterValue)
                .And.BeOnOrBefore(maxDateFilterValue);
        });
    }

    [Fact]
    public async Task Get_credits_ordered_by_amount_yields_ordered_Credits()
    {
        var home = (await _homes.GetHomes()).Content!.First();

        var query = new FilteredCreditsQuery
        {
            PageNumber = 1,
            PageSize = 25,
            OrderBy = nameof(CreditDigest.Amount),
            SortDirection = SortDirection.Asc
        };

        var credits = (await _homeCredits.GetHomeCredits(home.Id, query)).Content!.Items;

        credits.Should().BeInAscendingOrder(x => x.Amount);

        credits = (await _homeCredits.GetHomeCredits(home.Id, query with { SortDirection = SortDirection.Desc }))
            .Content!.Items;

        credits.Should().BeInDescendingOrder(x => x.Amount);
    }
}

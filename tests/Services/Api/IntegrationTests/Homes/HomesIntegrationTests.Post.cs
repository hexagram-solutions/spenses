using System.Net;
using Hexagrams.Extensions.Common.Serialization;
using Microsoft.AspNetCore.Mvc;
using Spenses.Shared.Models.Homes;
using Spenses.Shared.Models.Members;
using Spenses.Shared.Models.Users;

namespace Spenses.Api.IntegrationTests.Homes;

public partial class HomesIntegrationTests
{
    [Fact]
    public async Task Post_creates_home_with_default_member()
    {
        var properties = new HomeProperties
        {
            Name = "sut",
            Description = "baz"
        };

        var createdHome = (await _homes.PostHome(properties)).Content!;
        createdHome.Should().BeEquivalentTo(properties, opts =>
            opts.ExcludingNestedObjects()
                .ExcludingMissingMembers());

        var retrievedHome = (await _homes.GetHome(createdHome.Id)).Content!;
        retrievedHome.Should().BeEquivalentTo(createdHome);

        var currentUser = fixture.VerifiedUser;

        retrievedHome.Members.Single().Should().BeEquivalentTo(new Member
        {
            Name = currentUser.DisplayName,
            ContactEmail = currentUser.Email,
            AvatarUrl = currentUser.AvatarUrl,
            DefaultSplitPercentage = 1m,
            Status = MemberStatus.Active,
            User = new User { Id = currentUser.Id, DisplayName = currentUser.DisplayName }
        }, opts => opts.Excluding(m => m.Id));

        await _homes.DeleteHome(createdHome.Id);
    }

    [Fact]
    public async Task Post_invalid_home_yields_bad_request()
    {
        var result = await _homes.PostHome(new HomeProperties());

        result.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var validationErrors = result.Error!.Content!.FromJson<ValidationProblemDetails>()!.Errors;

        validationErrors.Should().ContainKey(nameof(Home.Name));
    }

    [Fact]
    public async Task Post_home_creates_home_with_default_expense_category()
    {
        var properties = new HomeProperties
        {
            Name = "sut",
            Description = "baz"
        };

        var createdHome = (await _homes.PostHome(properties)).Content!;

        var categories = (await _expenseCategories.GetExpenseCategories(createdHome.Id)).Content!;

        categories.Single().IsDefault.Should().BeTrue();

        await _homes.DeleteHome(createdHome.Id);
    }
}

using System.Net;
using Hexagrams.Extensions.Common.Serialization;
using Microsoft.AspNetCore.Mvc;
using Spenses.Application.Models.Homes;

namespace Spenses.Api.IntegrationTests.Homes;

public partial class HomesIntegrationTests
{
    [Fact]
    public async Task Post_creates_home()
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

        retrievedHome.Members.Should().HaveCount(1);

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
}

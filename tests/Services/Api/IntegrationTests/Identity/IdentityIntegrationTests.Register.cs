using System.Net;
using Bogus;
using Refit;
using Spenses.Shared.Models.Identity;

namespace Spenses.Api.IntegrationTests.Identity;

public partial class IdentityIntegrationTests
{
    [Fact]
    public async Task Register_creates_user_with_unverified_email()
    {
        var request = new RegisterRequest
        {
            Email = "hmccringleberry@psu.edu",
            Password = new Faker().Internet.Password(),
            DisplayName = "Hingle McCringleberry"
        };

        var response = await fixture.Register(request);

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var registeredUser = response.Content!;

        registeredUser.Id.Should().NotBeEmpty();
        registeredUser.Email.Should().Be(request.Email);
        registeredUser.DisplayName.Should().Be(request.DisplayName);
        registeredUser.EmailVerified.Should().BeFalse();

        await fixture.DeleteUser(registeredUser.Email);
    }

    [Fact]
    public async Task Register_with_email_as_password_yields_identity_error()
    {
        var request = new RegisterRequest
        {
            Email = "hmccringleberry@psu.edu",
            Password = "hmccringleberry@psu.edu",
            DisplayName = "Hingle McCringleberry"
        };

        var response = await fixture.Register(request);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var problemDetails = await response.Error!.GetContentAsAsync<ProblemDetails>();

        problemDetails!.Errors.Should().ContainKey(IdentityErrors.Password.EmailAsPassword);
    }

    [Fact]
    public async Task Register_with_pwned_password_yields_identity_error()
    {
        var request = new RegisterRequest
        {
            Email = "hmccringleberry@psu.edu",
            Password = "1234567890",
            DisplayName = "Hingle McCringleberry"
        };

        var response = await fixture.Register(request);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var problemDetails = await response.Error!.GetContentAsAsync<ProblemDetails>();

        problemDetails!.Errors.Should().ContainKey(IdentityErrors.Password.PwnedPassword);
    }

    [Fact]
    public async Task Register_with_duplicate_email_yields_identity_error()
    {
        var request = new RegisterRequest
        {
            Email = "hmccringleberry@psu.edu",
            Password = new Faker().Internet.Password(),
            DisplayName = "Hingle McCringleberry"
        };

        await fixture.Register(request);
        var response = await fixture.Register(request);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var problemDetails = await response.Error!.GetContentAsAsync<ProblemDetails>();

        problemDetails!.Errors.Should().ContainKey(IdentityErrors.Register.DuplicateUserName);

        await fixture.DeleteUser(request.Email);
    }

    [Fact]
    public async Task Register_with_invalid_values_yields_bad_request()
    {
        var request = new RegisterRequest
        {
            Email = "foobar",
            Password = "hunter2",
            DisplayName = string.Empty
        };

        var response = await fixture.Register(request);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var problemDetails = await response.Error!.GetContentAsAsync<ProblemDetails>();

        problemDetails!.Errors.Should().ContainKey(nameof(RegisterRequest.Email));
        problemDetails.Errors.Should().ContainKey(nameof(RegisterRequest.DisplayName));
        problemDetails.Errors.Should().ContainKey(nameof(RegisterRequest.Password));
    }
}

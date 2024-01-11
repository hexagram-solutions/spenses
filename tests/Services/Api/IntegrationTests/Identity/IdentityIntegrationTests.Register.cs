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
            Name = "Hingle McCringleberry"
        };

        var response = await _identityApi.Register(request);

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var registeredUser = response.Content!;

        registeredUser.Email.Should().Be(request.Email);
        registeredUser.NickName.Should().Be(request.Name);
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
            Name = "Hingle McCringleberry"
        };

        var response = await _identityApi.Register(request);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var problemDetails = await response.Error!.GetContentAsAsync<ProblemDetails>();

        problemDetails!.Errors.Should().ContainKey(IdentityErrors.EmailAsPassword);
    }

    [Fact]
    public async Task Register_with_pwned_password_yields_identity_error()
    {
        var request = new RegisterRequest
        {
            Email = "hmccringleberry@psu.edu",
            Password = "1234567890",
            Name = "Hingle McCringleberry"
        };

        var response = await _identityApi.Register(request);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var problemDetails = await response.Error!.GetContentAsAsync<ProblemDetails>();

        problemDetails!.Errors.Should().ContainKey(IdentityErrors.PwnedPassword);
    }

    [Fact]
    public async Task Register_with_duplicate_email_yields_identity_error()
    {
        var request = new RegisterRequest
        {
            Email = "hmccringleberry@psu.edu",
            Password = new Faker().Internet.Password(),
            Name = "Hingle McCringleberry"
        };

        await _identityApi.Register(request);
        var response = await _identityApi.Register(request);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var problemDetails = await response.Error!.GetContentAsAsync<ProblemDetails>();

        problemDetails!.Errors.Should().ContainKey(IdentityErrors.DuplicateUserName);

        await fixture.DeleteUser(request.Email);
    }

    [Fact]
    public async Task Register_with_invalid_values_yields_bad_request()
    {
        var request = new RegisterRequest
        {
            Email = "foobar",
            Password = "hunter2",
            Name = string.Empty
        };

        var response = await _identityApi.Register(request);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var problemDetails = await response.Error!.GetContentAsAsync<ProblemDetails>();

        problemDetails!.Errors.Should().ContainKey(nameof(RegisterRequest.Email));
        problemDetails.Errors.Should().ContainKey(nameof(RegisterRequest.Name));
        problemDetails.Errors.Should().ContainKey(nameof(RegisterRequest.Password));
    }
}

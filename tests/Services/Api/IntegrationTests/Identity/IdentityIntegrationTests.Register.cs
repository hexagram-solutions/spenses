using System.Net;
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
            Email = _faker.Internet.Email(),
            Password = _faker.Internet.Password(),
            DisplayName = "Hingle McCringleberry"
        };

        var response = await Register(request);

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var registeredUser = response.Content!;

        registeredUser.Id.Should().NotBeEmpty();
        registeredUser.Email.Should().Be(request.Email);
        registeredUser.DisplayName.Should().Be(request.DisplayName);
        registeredUser.EmailVerified.Should().BeFalse();
    }

    [Fact]
    public async Task Register_with_email_as_password_yields_identity_error()
    {
        var email = _faker.Internet.Email();

        var request = new RegisterRequest
        {
            Email = email,
            Password = email,
            DisplayName = "Hingle McCringleberry"
        };

        var response = await Register(request);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var problemDetails = await response.Error!.GetContentAsAsync<ProblemDetails>();

        problemDetails!.Errors.Should().ContainKey(IdentityErrors.Password.EmailAsPassword);
    }

    [Fact]
    public async Task Register_with_pwned_password_yields_identity_error()
    {
        var request = new RegisterRequest
        {
            Email = _faker.Internet.Email(),
            Password = "1234567890",
            DisplayName = "Hingle McCringleberry"
        };

        var response = await Register(request);

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
            Password = _faker.Internet.Password(),
            DisplayName = "Hingle McCringleberry"
        };

        await Register(request);
        var response = await Register(request);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var problemDetails = await response.Error!.GetContentAsAsync<ProblemDetails>();

        problemDetails!.Errors.Should().ContainKey(IdentityErrors.Register.DuplicateUserName);
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

        var response = await Register(request);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var problemDetails = await response.Error!.GetContentAsAsync<ProblemDetails>();

        problemDetails!.Errors.Should().ContainKey(nameof(RegisterRequest.Email));
        problemDetails.Errors.Should().ContainKey(nameof(RegisterRequest.DisplayName));
        problemDetails.Errors.Should().ContainKey(nameof(RegisterRequest.Password));
    }
}

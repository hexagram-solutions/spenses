using System.Net;
using Spenses.Shared.Models.Identity;

namespace Spenses.Api.IntegrationTests.Identity;

public partial class IdentityIntegrationTests
{
    [Fact]
    public async Task Login_with_valid_credentials_yields_success()
    {
        var registerRequest = new RegisterRequest
        {
            Email = _faker.Internet.Email(),
            Password = _faker.Internet.Password(),
            DisplayName = "DONKEY TEETH"
        };

        await AuthFixture.Register(registerRequest, true);

        var response = await AuthFixture.Login(new LoginRequest
        {
            Email = registerRequest.Email,
            Password = registerRequest.Password
        });

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        response.Content!.Succeeded.Should().BeTrue();
    }

    [Fact]
    public async Task Login_with_invalid_credentials_yields_unauthorized()
    {
        var response = await _identityApi.Login(new LoginRequest
        {
            Email = AuthFixture.CurrentUser.Email,
            Password = "foo"
        });

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);

        var errorContent = await response.Error!.GetContentAsAsync<LoginResult>();

        errorContent!.Succeeded.Should().BeFalse();
    }

    [Fact]
    public async Task Login_with_unverified_email_yields_unauthorized()
    {
        var registerRequest = new RegisterRequest
        {
            Email = _faker.Internet.Email(),
            Password = _faker.Internet.Password(),
            DisplayName = "DONKEY TEETH"
        };

        await AuthFixture.Register(registerRequest);

        var response = await AuthFixture.Login(new LoginRequest
        {
            Email = registerRequest.Email,
            Password = registerRequest.Password
        });

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);

        var errorContent = await response.Error!.GetContentAsAsync<LoginResult>();

        errorContent!.Succeeded.Should().BeFalse();
        errorContent.IsNotAllowed.Should().BeTrue();
    }
}

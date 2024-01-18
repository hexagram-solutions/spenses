using System.Net;
using Bogus;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Net.Http.Headers;
using Refit;
using Spenses.Api.IntegrationTests.Identity;
using Spenses.Client.Http;
using Spenses.Resources.Relational.Models;
using Spenses.Shared.Models.Identity;
using Spenses.Shared.Models.Me;

namespace Spenses.Api.IntegrationTests.Me;

public partial class MeIntegrationTests
{
    [Fact]
    public async Task Change_email_for_verified_user_changes_and_verifies_email()
    {
        await using var identityFixture = new IdentityWebApplicationFixture<Program>();

        await identityFixture.InitializeAsync();

        using var httpClient = identityFixture.WebApplicationFactory.CreateClient();

        var identityApi = RestService.For<IIdentityApi>(httpClient);

        // Register a new user
        var registerRequest = new RegisterRequest
        {
            DisplayName = "Quackadilly Blip",
            Email = "quackadilly.blip@auburn.edu",
            Password = new Faker().Internet.Password()
        };

        var registrationResponse = await identityApi.Register(registerRequest);

        registrationResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var (verificationUserId, verificationCode, _) = identityFixture.GetVerificationParametersForEmail(registerRequest.Email);

        var initialVerificationResponse = await identityApi.VerifyEmail(new VerifyEmailRequest(verificationUserId, verificationCode));

        initialVerificationResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        // Log in as the new user
        var loginResponse = await identityApi.Login(new LoginRequest { Email = registerRequest.Email, Password = registerRequest.Password });

        var (authCookieName, authCookieValue) = GetAuthCookieFromResponse(loginResponse);

        using var authenticatedClient = identityFixture.WebApplicationFactory.CreateClient();
        authenticatedClient.DefaultRequestHeaders.Add("Cookie", $"{authCookieName}={authCookieValue}");

        var authenticatedMeApi = RestService.For<IMeApi>(authenticatedClient);

        // Request the email change
        var expectedEmail = "quackadilly.blip2@auburn.edu";

        var changeEmailResponse = await authenticatedMeApi.ChangeEmail(new ChangeEmailRequest { NewEmail = expectedEmail });

        changeEmailResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var (userId, code, newEmail) = identityFixture.GetVerificationParametersForEmail(expectedEmail);

        var verificationResponse = await identityApi.VerifyEmail(new VerifyEmailRequest(userId, code, newEmail));

        verificationResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        // Ensure the user was logged out
        var (_, logoutCookieValue) = GetAuthCookieFromResponse(verificationResponse);

        logoutCookieValue.Should().BeEmpty();

        var userManager = fixture.WebApplicationFactory.Services.GetRequiredService<UserManager<ApplicationUser>>();

        var user = await userManager.FindByIdAsync(userId);

        user!.UserName.Should().Be(expectedEmail);
        user.Email.Should().Be(expectedEmail);
        user.EmailConfirmed.Should().Be(true);

        await identityFixture.DeleteUser(expectedEmail);
    }

    private (string authCookieName, string authCookieValue) GetAuthCookieFromResponse(IApiResponse response)
    {
        var setCookieHeader = response.Headers.GetValues("Set-Cookie")
            .Single(v => v.StartsWith(".AspNetCore.Identity.Application"));

        var setCookieHeaderValue = SetCookieHeaderValue.Parse(setCookieHeader);

        return (setCookieHeaderValue.Name.ToString(), setCookieHeaderValue.Value.ToString());
    }
}

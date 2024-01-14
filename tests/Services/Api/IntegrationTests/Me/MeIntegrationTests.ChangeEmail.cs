using System.Net;
using Bogus;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
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
            Name = "Quackadilly Blip",
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

        var authCookieValue = loginResponse.Headers.GetValues("Set-Cookie").Single();
        var cookieData = GetCookieData(authCookieValue);
        //var authCookie = CookieHeaderValue.Parse(authCookieValue);

        var handler = new HttpClientHandler();
        handler.CookieContainer.Add(httpClient.BaseAddress!, new Cookie(cookieData.First().Key, cookieData.First().Value));

        using var authenticatedClient = identityFixture.WebApplicationFactory.CreateClient();
        authenticatedClient.DefaultRequestHeaders.Add("Cookie", $"{cookieData.First().Key}={cookieData.First().Value}");

        var authenticatedMeApi = RestService.For<IMeApi>(authenticatedClient);

        // Request the email change
        var expectedEmail = "quackadilly.blip2@auburn.edu";

        try
        {
            var response = await authenticatedMeApi.ChangeEmail(new ChangeEmailRequest { NewEmail = expectedEmail });

            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }
        catch (HttpRequestException e)
        {
            Console.WriteLine(e);
            throw;
        }

        var (userId, code, newEmail) = identityFixture.GetVerificationParametersForEmail(expectedEmail);

        var verificationResponse = await identityApi.VerifyEmail(new VerifyEmailRequest(userId, code, newEmail));

        //todo: user should be logged out

        var userManager = fixture.WebApplicationFactory.Services.GetRequiredService<UserManager<ApplicationUser>>();

        var user = await userManager.FindByIdAsync(userId);

        user!.UserName.Should().Be(expectedEmail);
        user.Email.Should().Be(expectedEmail);
        user.EmailConfirmed.Should().Be(true);

        await identityFixture.DeleteUser(expectedEmail);
    }

    private IDictionary<string, string> GetCookieData(string SetCookieValue)
    {
        var cookieDictionary = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        var values = SetCookieValue.TrimEnd(';').Split(';');
        foreach (var parts in values.Select(c => c.Split(new[] { '=' }, 2)))
        {
            var cookieName = parts[0].Trim();
            string cookieValue;

            if (parts.Length == 1)
            {
                //Cookie attribute
                cookieValue = string.Empty;
            }
            else
            {
                cookieValue = parts[1];
            }

            cookieDictionary[cookieName] = cookieValue;
        }

        return cookieDictionary;
    }
}

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
        var identityApi = RestService.For<IIdentityApi>(fixture.CreateClient());

        // Register a new user
        var registerRequest = new RegisterRequest
        {
            DisplayName = "Quackadilly Blip",
            Email = "quackadilly.blip@auburn.edu",
            Password = new Faker().Internet.Password()
        };

        await fixture.Register(registerRequest, true);

        await fixture.Login(new LoginRequest
        {
            Email = registerRequest.Email,
            Password = registerRequest.Password
        });

        var meApi = RestService.For<IMeApi>(fixture.CreateClient());

        // Request the email change
        var expectedEmail = "quackadilly.blip2@auburn.edu";

        var changeEmailResponse = await meApi.ChangeEmail(new ChangeEmailRequest { NewEmail = expectedEmail });

        changeEmailResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var (userId, code, newEmail) = fixture.GetVerificationParametersForEmail(expectedEmail);

        var verificationResponse = await identityApi.VerifyEmail(new VerifyEmailRequest(userId, code, newEmail));

        verificationResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        // Ensure the user was logged out
        var meResponse = await meApi.GetMe();

        meResponse.StatusCode.Should().Be(HttpStatusCode.Unauthorized);

        // Verify that the user's email was changed in the database
        var userManager = fixture.WebApplicationFactory.Services.GetRequiredService<UserManager<ApplicationUser>>();

        var user = await userManager.FindByIdAsync(userId);

        user!.UserName.Should().Be(expectedEmail);
        user.Email.Should().Be(expectedEmail);
        user.EmailConfirmed.Should().Be(true);

        await fixture.DeleteUser(expectedEmail);
    }
}

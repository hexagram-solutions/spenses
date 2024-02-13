using System.Net;
using Bogus;
using Spenses.Client.Http;
using Spenses.Shared.Models.Identity;
using Spenses.Shared.Models.Me;

namespace Spenses.Api.IntegrationTests.Me;

public partial class MeIntegrationTests
{
    [Fact]
    public async Task Change_email_for_verified_user_changes_and_verifies_email()
    {
        var identityApi = CreateApiClient<IIdentityApi>();

        // Register a new user
        var registerRequest = new RegisterRequest
        {
            DisplayName = "Quackadilly Blip",
            Email = "quackadilly.blip@auburn.edu",
            Password = new Faker().Internet.Password()
        };

        await AuthFixture.Register(registerRequest, true);

        await AuthFixture.Login(new LoginRequest
        {
            Email = registerRequest.Email,
            Password = registerRequest.Password
        });

        var meApi = CreateApiClient<IMeApi>();

        // Request the email change
        var expectedEmail = "quackadilly.blip2@auburn.edu";

        var changeEmailResponse = await meApi.ChangeEmail(new ChangeEmailRequest { NewEmail = expectedEmail });

        changeEmailResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var (userId, code, newEmail) = AuthFixture.GetVerificationParametersForEmail(expectedEmail);

        var verificationResponse = await identityApi.VerifyEmail(new VerifyEmailRequest(userId, code, newEmail));

        verificationResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        // Ensure the user was logged out
        var meResponse = await meApi.GetMe();

        meResponse.StatusCode.Should().Be(HttpStatusCode.Unauthorized);

        // Verify that the user's email was changed in the database
        await DatabaseFixture.ExecuteDbContextAction(async db =>
        {
            var user = await db.Users.FindAsync(userId);

            user!.UserName.Should().Be(expectedEmail);
            user.Email.Should().Be(expectedEmail);
            user.EmailConfirmed.Should().Be(true);
        });
    }
}

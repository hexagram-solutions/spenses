using System.Net;
using Bogus;
using Refit;
using Spenses.Shared.Models.Identity;

namespace Spenses.Api.IntegrationTests.Identity;
public partial class IdentityIntegrationTests
{
    [Fact]
    public async Task Reset_password_updates_user_password()
    {
        var verifiedUser = fixture.VerifiedUser;

        await _identityApi.ForgotPassword(new ForgotPasswordRequest(verifiedUser.Email));

        var (email, code) = fixture.GetPasswordResetParametersForEmail(verifiedUser.Email);

        var newPassword = new Faker().Internet.Password();

        var resetResponse = await _identityApi.ResetPassword(new ResetPasswordRequest(email, code, newPassword));

        resetResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var loginResponse = await _identityApi.Login(new LoginRequest
        {
            Email = verifiedUser.Email, Password = newPassword
        });

        loginResponse.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Reset_password_with_invalid_parameters_yields_bad_request()
    {
        var response = await _identityApi.ResetPassword(new ResetPasswordRequest("foo", string.Empty, "baz"));

        var problemDetails = await response.Error!.GetContentAsAsync<ProblemDetails>();

        problemDetails!.Errors.Should().ContainKey(nameof(ResetPasswordRequest.Email));
        problemDetails.Errors.Should().ContainKey(nameof(ResetPasswordRequest.ResetCode));
        problemDetails.Errors.Should().ContainKey(nameof(ResetPasswordRequest.NewPassword));
    }

    [Fact]
    public async Task Reset_password_with_email_as_password_yields_identity_error()
    {
        var verifiedUser = fixture.VerifiedUser;

        await _identityApi.ForgotPassword(new ForgotPasswordRequest(verifiedUser.Email));

        var (email, code) = fixture.GetPasswordResetParametersForEmail(verifiedUser.Email);

        var newPassword = verifiedUser.Email;

        var resetResponse = await _identityApi.ResetPassword(new ResetPasswordRequest(email, code, newPassword));

        resetResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var problemDetails = await resetResponse.Error!.GetContentAsAsync<ProblemDetails>();

        problemDetails!.Errors.Should().ContainKey(IdentityErrors.EmailAsPassword);
    }

    [Fact]
    public async Task Reset_password_with_invalid_code_yields_identity_error()
    {
        var verifiedUser = fixture.VerifiedUser;

        var resetResponse = await _identityApi.ResetPassword(
            new ResetPasswordRequest(verifiedUser.Email, "foo", new Faker().Internet.Password()));

        resetResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var problemDetails = await resetResponse.Error!.GetContentAsAsync<ProblemDetails>();

        problemDetails!.Errors.Should().ContainKey(IdentityErrors.InvalidToken);
    }

    [Fact]
    public async Task Reset_password_for_non_existent_user_yields_identity_error()
    {
        var resetResponse = await _identityApi.ResetPassword(
            new ResetPasswordRequest("quatro.quatro@sjsu.edu", "foo", new Faker().Internet.Password()));

        resetResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var problemDetails = await resetResponse.Error!.GetContentAsAsync<ProblemDetails>();

        problemDetails!.Errors.Should().ContainKey(IdentityErrors.InvalidToken);
    }
}

using System.Net;
using Refit;
using Spenses.Shared.Models.Identity;

namespace Spenses.Api.IntegrationTests.Identity;

public partial class IdentityIntegrationTests
{
    [Fact]
    public async Task Reset_password_updates_user_password()
    {
        var registerRequest = new RegisterRequest
        {
            Email = "hmccringleberry@psu.edu",
            Password = _faker.Internet.Password(),
            DisplayName = "Hingle McCringleberry"
        };

        var registerResponse = await AuthFixture.Register(registerRequest, true);

        var verifiedUser = registerResponse.Content!;

        await _identityApi.ForgotPassword(new ForgotPasswordRequest { Email = verifiedUser.Email });

        var (email, code) = AuthFixture.GetPasswordResetParametersForEmail(verifiedUser.Email);

        var newPassword = _faker.Internet.Password();

        var resetResponse = await _identityApi.ResetPassword(new ResetPasswordRequest
        {
            Email = email,
            NewPassword = newPassword,
            ResetCode = code
        });

        resetResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var loginResponse = await _identityApi.Login(new LoginRequest
        {
            Email = verifiedUser.Email,
            Password = newPassword
        });

        loginResponse.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Reset_password_with_invalid_parameters_yields_bad_request()
    {
        var response = await _identityApi.ResetPassword(new ResetPasswordRequest
        {
            Email = "foo",
            NewPassword = "bar",
            ResetCode = string.Empty
        });

        var problemDetails = await response.Error!.GetContentAsAsync<ProblemDetails>();

        problemDetails!.Errors.Should().ContainKey(nameof(ResetPasswordRequest.Email));
        problemDetails.Errors.Should().ContainKey(nameof(ResetPasswordRequest.ResetCode));
        problemDetails.Errors.Should().ContainKey(nameof(ResetPasswordRequest.NewPassword));
    }

    [Fact]
    public async Task Reset_password_with_email_as_password_yields_identity_error()
    {
        var verifiedUser = AuthFixture.CurrentUser;

        await _identityApi.ForgotPassword(new ForgotPasswordRequest { Email = verifiedUser.Email });

        var (email, code) = AuthFixture.GetPasswordResetParametersForEmail(verifiedUser.Email);

        var newPassword = verifiedUser.Email;

        var resetResponse = await _identityApi.ResetPassword(new ResetPasswordRequest
        {
            Email = email,
            NewPassword = newPassword,
            ResetCode = code
        });

        resetResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var problemDetails = await resetResponse.Error!.GetContentAsAsync<ProblemDetails>();

        problemDetails!.Errors.Should().ContainKey(IdentityErrors.Password.EmailAsPassword);
    }

    [Fact]
    public async Task Reset_password_with_invalid_code_yields_identity_error()
    {
        var verifiedUser = AuthFixture.CurrentUser;

        var resetResponse = await _identityApi.ResetPassword(new ResetPasswordRequest
        {
            Email = verifiedUser.Email,
            ResetCode = "foo",
            NewPassword = _faker.Internet.Password(),
        });

        resetResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var problemDetails = await resetResponse.Error!.GetContentAsAsync<ProblemDetails>();

        problemDetails!.Errors.Should().ContainKey(IdentityErrors.EmailVerification.InvalidToken);
    }

    [Fact]
    public async Task Reset_password_for_non_existent_user_yields_identity_error()
    {
        var resetResponse = await _identityApi.ResetPassword(new ResetPasswordRequest
        {
            Email = "quatro.quatro@sjsu.edu",
            ResetCode = "foo",
            NewPassword = _faker.Internet.Password()
        });

        resetResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var problemDetails = await resetResponse.Error!.GetContentAsAsync<ProblemDetails>();

        problemDetails!.Errors.Should().ContainKey(IdentityErrors.EmailVerification.InvalidToken);
    }
}

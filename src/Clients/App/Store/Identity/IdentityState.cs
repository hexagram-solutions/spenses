using Fluxor;

namespace Spenses.App.Store.Identity;

[FeatureState(Name = "Identity", CreateInitialStateMethodName = nameof(Initialize))]
public record IdentityState
{
    private static IdentityState Initialize()
    {
        return new IdentityState();
    }

    public bool LoginRequesting { get; init; }

    public bool RegistrationRequesting { get; init; }

    public bool EmailVerificationRequesting { get; init; }

    public bool ResendVerificationEmailRequesting { get; init; }

    public bool LogoutRequesting { get; init; }

    public bool ForgotPasswordRequesting { get; init; }

    public string[] Errors { get; init; } = [];
}

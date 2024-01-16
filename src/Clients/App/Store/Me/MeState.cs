using Fluxor;
using Spenses.Shared.Models.Me;

namespace Spenses.App.Store.Me;

[FeatureState(Name = "Me", CreateInitialStateMethodName = nameof(Initialize))]
public record MeState
{
    private static MeState Initialize()
    {
        return new MeState();
    }

    public bool CurrentUserRequesting { get; init; }

    public bool CurrentUserUpdating { get; init; }

    public bool ChangeEmailRequesting { get; init; }

    public bool ChangePasswordRequesting { get; init; }

    public CurrentUser? CurrentUser { get; init; }

    public string[] ChangeEmailErrors { get; init; } = [];

    public string[] ChangePasswordErrors { get; init; } = [];
}

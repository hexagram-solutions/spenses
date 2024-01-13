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

    public CurrentUser? CurrentUser { get; init; }
}

using Fluxor;
using Spenses.Shared.Models.Homes;

namespace Spenses.Web.Client.Store.Homes;

[FeatureState(Name = "Homes", CreateInitialStateMethodName = nameof(Initialize))]
public record HomesState
{
    private static HomesState Initialize()
    {
        return new HomesState();
    }

    public Home? CurrentHome { get; init; }

    public Home[] Homes { get; init; } = [];

    public bool HomesRequesting { get; init; }

    public bool HomeRequesting { get; init; }

    public bool HomeCreating { get; init; }

    public bool HomeUpdating { get; init; }
}

using Fluxor;
using Spenses.Application.Models.Homes;

namespace Spenses.Client.Web.Store.Homes;

[FeatureState(Name = "Homes", CreateInitialStateMethodName = nameof(Initialize))]
public record HomesState
{
    private static HomesState Initialize()
    {
        return new HomesState();
    }

    public Home? CurrentHome { get; init; }

    public IEnumerable<Home> Homes { get; init; } = Enumerable.Empty<Home>();

    public bool HomesRequesting { get; init; }

    public bool HomeRequesting { get; init; }

    public bool HomeCreating { get; init; }

    public bool HomeUpdating { get; init; }

    // TODO: enhanced error model
    public string? Error { get; init; }
}

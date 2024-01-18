using Fluxor;
using Spenses.App.Infrastructure;

namespace Spenses.App.Store.Shared;

[FeatureState(Name = "Shared", CreateInitialStateMethodName = nameof(Initialize))]
public record SharedState
{
    private static SharedState Initialize()
    {
        return new SharedState();
    }

    public ApplicationError? Error { get; set; }
}

using Fluxor;
using Spenses.Client.Web.Infrastructure;

namespace Spenses.Client.Web.Store.Shared;

[FeatureState(Name = "Shared", CreateInitialStateMethodName = nameof(Initialize))]
public record SharedState
{
    private static SharedState Initialize()
    {
        return new SharedState();
    }

    public ApplicationError? Error { get; set; }
}

public record ApplicationErrorAction(ApplicationError Error);

public static class Reducers
{
    [ReducerMethod]
    public static SharedState ReduceApiError(SharedState state, ApplicationErrorAction action)
    {
        return state with { Error = action.Error };
    }
}

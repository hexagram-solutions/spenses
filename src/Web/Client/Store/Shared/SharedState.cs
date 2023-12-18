using Fluxor;
using Spenses.Web.Client.Infrastructure;

namespace Spenses.Web.Client.Store.Shared;

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
        return new SharedState { Error = action.Error };
    }
}

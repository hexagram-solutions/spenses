using Fluxor;

namespace Spenses.App.Store.Shared;

public static class Reducers
{
    [ReducerMethod]
    public static SharedState ReduceApplicationError(SharedState state, ApplicationErrorAction action)
    {
        return state with { Error = action.Error };
    }
}

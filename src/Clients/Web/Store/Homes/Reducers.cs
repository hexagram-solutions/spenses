using Fluxor;

namespace Spenses.Client.Web.Store.Homes;

public static class Reducers
{
    [ReducerMethod]
    public static HomesState ReduceHomesRequested(HomesState state, HomesRequestedAction _) =>
        state with { HomesRequesting = true };

    [ReducerMethod]
    public static HomesState ReduceHomesReceived(HomesState state, HomesReceivedAction action) =>
        state with
        {
            HomesRequesting = false,
            Homes = action.Homes
        };

    [ReducerMethod]
    public static HomesState ReduceHomesRequestFailed(HomesState state, HomesRequestFailedAction action) =>
        state with
        {
            HomesRequesting = false,
            Error = action.Message
        };
}

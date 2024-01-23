using Fluxor;
using Hexagrams.Extensions.Common;

namespace Spenses.App.Store.Homes;

public static class Reducers
{
    [ReducerMethod]
    public static HomesState ReduceHomesRequested(HomesState state, HomesRequestedAction _)
    {
        return state with { HomesRequesting = true };
    }

    [ReducerMethod]
    public static HomesState ReduceHomesReceived(HomesState state, HomesReceivedAction action)
    {
        return state with { HomesRequesting = false, Homes = action.Homes };
    }

    [ReducerMethod]
    public static HomesState ReduceHomesRequestFailed(HomesState state, HomesRequestFailedAction _)
    {
        return state with { HomesRequesting = false };
    }

    [ReducerMethod]
    public static HomesState ReduceHomeRequested(HomesState state, HomeRequestedAction _)
    {
        return state with { HomeRequesting = true };
    }

    [ReducerMethod]
    public static HomesState ReduceHomeReceived(HomesState state, HomeReceivedAction action)
    {
        return state with { HomeRequesting = false, CurrentHome = action.Home };
    }

    [ReducerMethod]
    public static HomesState ReduceHomeRequestedFailed(HomesState state, HomeRequestFailedAction _)
    {
        return state with { HomeRequesting = false };
    }

    [ReducerMethod]
    public static HomesState ReduceHomeCreated(HomesState state, HomeCreatedAction _)
    {
        return state with { HomeCreating = true };
    }

    [ReducerMethod]
    public static HomesState ReduceHomeCreationSucceeded(HomesState state, HomeCreationSucceededAction action)
    {
        return state with { HomeCreating = false, CurrentHome = action.Home };
    }

    [ReducerMethod]
    public static HomesState ReduceHomeCreationFailed(HomesState state, HomeCreationFailedAction _)
    {
        return state with { HomeCreating = false };
    }

    [ReducerMethod]
    public static HomesState ReduceHomeUpdated(HomesState state, HomeUpdatedAction _)
    {
        return state with { HomeUpdating = true };
    }

    [ReducerMethod]
    public static HomesState ReduceHomeUpdateSucceeded(HomesState state, HomeUpdateSucceededAction action)
    {
        var originalHome = state.Homes.Single(h => h.Id == action.Home.Id);

        return state with
        {
            HomeUpdating = false,
            CurrentHome = action.Home,
            Homes = state.Homes.Replace(originalHome, action.Home).ToArray()
        };
    }

    [ReducerMethod]
    public static HomesState ReduceHomeUpdateFailed(HomesState state, HomeUpdateFailedAction _)
    {
        return state with { HomeUpdating = false };
    }

    [ReducerMethod]
    public static HomesState ReduceBalanceBreakdownRequested(HomesState state, BalanceBreakdownRequestedAction _)
    {
        return state with { BalanceBreakdownRequesting = true };
    }

    [ReducerMethod]
    public static HomesState ReduceBalanceBreakdownReceived(HomesState state, BalanceBreakdownReceivedAction action)
    {
        return state with { BalanceBreakdownRequesting = false, BalanceBreakdown = action.BalanceBreakdown };
    }

    [ReducerMethod]
    public static HomesState ReduceBalanceBreakdownRequestFailed(HomesState state,
        BalanceBreakdownRequestFailedAction _)
    {
        return state with { BalanceBreakdownRequesting = false };
    }
}

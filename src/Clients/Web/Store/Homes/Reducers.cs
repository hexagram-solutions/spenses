using DynamicData;
using Fluxor;
using Spenses.Application.Models.Homes;

namespace Spenses.Client.Web.Store.Homes;

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
    public static HomesState ReduceHomesRequestFailed(HomesState state, HomesRequestFailedAction action)
    {
        return state with { HomesRequesting = false, Error = action.Error };
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
    public static HomesState ReduceHomeRequestedFailed(HomesState state, HomeRequestFailedAction action)
    {
        return state with { HomeRequesting = false, Error = action.Error };
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
    public static HomesState ReduceHomeCreationFailed(HomesState state, HomeCreationFailedAction action)
    {
        return state with { HomesRequesting = false, Error = action.Error };
    }

    [ReducerMethod]
    public static HomesState ReduceHomeUpdated(HomesState state, HomeUpdatedAction _)
    {
        return state with { HomeUpdating = true };
    }

    [ReducerMethod]
    public static HomesState ReduceHomeUpdateSucceeded(HomesState state, HomeUpdateSucceededAction action)
    {
        var homes = new List<Home>(state.Homes);

        var originalHome = homes.Single(h => h.Id == action.Home.Id);

        homes.Replace(originalHome, action.Home);

        return state with { HomeUpdating = false, CurrentHome = action.Home, Homes = homes.ToArray() };
    }

    [ReducerMethod]
    public static HomesState ReduceHomeUpdateFailed(HomesState state, HomeUpdateFailedAction action)
    {
        return state with { HomesRequesting = false, Error = action.Error };
    }
}

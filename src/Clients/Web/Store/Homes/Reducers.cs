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

    [ReducerMethod]
    public static HomesState ReduceHomeRequested(HomesState state, HomeRequestedAction _) =>
        state with { HomeRequesting = true };

    [ReducerMethod]
    public static HomesState ReduceHomeReceived(HomesState state, HomeReceivedAction action) =>
        state with
        {
            HomeRequesting = false,
            CurrentHome = action.Home
        };

    [ReducerMethod]
    public static HomesState ReduceHomeRequestedFailed(HomesState state, HomeRequestFailedAction action) =>
        state with
        {
            HomeRequesting = false,
            Error = action.Message
        };

    [ReducerMethod]
    public static HomesState ReduceHomeCreated(HomesState state, HomeCreatedAction _) =>
        state with { HomeCreating = true };

    [ReducerMethod]
    public static HomesState ReduceHomeCreationSucceeded(HomesState state, HomeCreationSucceededAction action) =>
        state with
        {
            HomeCreating = false,
            CurrentHome = action.Home
        };

    [ReducerMethod]
    public static HomesState ReduceHomeCreationFailed(HomesState state, HomeCreationFailedAction action) =>
        state with
        {
            HomesRequesting = false,
            Error = action.Message
        };

    [ReducerMethod]
    public static HomesState ReduceHomeUpdated(HomesState state, HomeUpdatedAction _) =>
        state with { HomeUpdating = true };

    [ReducerMethod]
    public static HomesState ReduceHomeUpdateSucceeded(HomesState state, HomeUpdateSucceededAction action) =>
        state with
        {
            HomeUpdating = false,
            CurrentHome = action.Home
        };

    [ReducerMethod]
    public static HomesState ReduceHomeUpdateFailed(HomesState state, HomeUpdateFailedAction action) =>
        state with
        {
            HomesRequesting = false,
            Error = action.Message
        };
}

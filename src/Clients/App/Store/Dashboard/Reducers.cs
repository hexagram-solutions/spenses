using Fluxor;

namespace Spenses.App.Store.Dashboard;

public static class Reducers
{
    [ReducerMethod]
    public static DashboardState ReduceDashboardPeriodChanged(DashboardState state, DashboardPeriodChangedAction action)
    {
        return state with { PeriodStart = action.Start, PeriodEnd = action.End };
    }

    [ReducerMethod]
    public static DashboardState ReduceBalanceBreakdownRequested(DashboardState state, BalanceBreakdownRequestedAction _)
    {
        return state with { BalanceBreakdownRequesting = true };
    }

    [ReducerMethod]
    public static DashboardState ReduceBalanceBreakdownReceived(DashboardState state, BalanceBreakdownReceivedAction action)
    {
        return state with { BalanceBreakdownRequesting = false, BalanceBreakdown = action.BalanceBreakdown };
    }

    [ReducerMethod]
    public static DashboardState ReduceBalanceBreakdownRequestFailed(DashboardState state,
        BalanceBreakdownRequestFailedAction _)
    {
        return state with { BalanceBreakdownRequesting = false };
    }
}

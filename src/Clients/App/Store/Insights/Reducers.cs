using Fluxor;

namespace Spenses.App.Store.Insights;

public static class Reducers
{
    [ReducerMethod]
    public static InsightsState ReduceExpensesOverTimeRequested(InsightsState state, ExpensesOverTimeRequestedAction _)
    {
        return state with { ExpensesOverTimeRequesting = true };
    }

    [ReducerMethod]
    public static InsightsState ReduceHomesReceived(InsightsState state, ExpensesOverTimeReceivedAction action)
    {
        return state with { ExpensesOverTimeRequesting = false, ExpensesOverTime = action.Items };
    }

    [ReducerMethod]
    public static InsightsState ReduceHomesRequestFailed(InsightsState state, ExpensesOverTimeRequestFailedAction _)
    {
        return state with { ExpensesOverTimeRequesting = false };
    }
}

using Fluxor;

namespace Spenses.Client.Web.Store.Expenses;

public static class Reducers
{
    [ReducerMethod]
    public static ExpensesState ReduceExpensesRequested(ExpensesState state, ExpensesRequestedAction _)
    {
        return state with { ExpensesRequesting = true };
    }

    [ReducerMethod]
    public static ExpensesState ReduceExpensesReceived(ExpensesState state, ExpensesReceivedAction action)
    {
        return state with { ExpensesRequesting = false, Expenses = action.Expenses };
    }

    [ReducerMethod]
    public static ExpensesState ReduceExpensesRequestFailed(ExpensesState state, ExpensesRequestFailedAction action)
    {
        return state with { ExpensesRequesting = false };
    }

    [ReducerMethod]
    public static ExpensesState ReduceExpenseRequested(ExpensesState state, ExpenseRequestedAction _)
    {
        return state with { ExpenseRequesting = true };
    }

    [ReducerMethod]
    public static ExpensesState ReduceExpenseReceived(ExpensesState state, ExpenseReceivedAction action)
    {
        return state with { ExpenseRequesting = false, CurrentExpense = action.Expense };
    }

    [ReducerMethod]
    public static ExpensesState ReduceExpenseRequestedFailed(ExpensesState state, ExpenseRequestFailedAction action)
    {
        return state with { ExpenseRequesting = false };
    }

    [ReducerMethod]
    public static ExpensesState ReduceExpenseCreated(ExpensesState state, ExpenseCreatedAction _)
    {
        return state with { ExpenseCreating = true };
    }

    [ReducerMethod]
    public static ExpensesState ReduceExpenseCreationSucceeded(ExpensesState state,
        ExpenseCreationSucceededAction action)
    {
        return state with { ExpenseCreating = false, CurrentExpense = action.Expense };
    }

    [ReducerMethod]
    public static ExpensesState ReduceExpenseCreationFailed(ExpensesState state, ExpenseCreationFailedAction action)
    {
        return state with { ExpenseCreating = false };
    }

    [ReducerMethod]
    public static ExpensesState ReduceExpenseUpdated(ExpensesState state, ExpenseUpdatedAction _)
    {
        return state with { ExpenseUpdating = true };
    }

    [ReducerMethod]
    public static ExpensesState ReduceExpenseUpdateSucceeded(ExpensesState state, ExpenseUpdateSucceededAction action)
    {
        return state with { ExpenseUpdating = false, CurrentExpense = action.Expense };
    }

    [ReducerMethod]
    public static ExpensesState ReduceExpenseUpdateFailed(ExpensesState state, ExpenseUpdateFailedAction action)
    {
        return state with { ExpenseUpdating = false };
    }

    [ReducerMethod]
    public static ExpensesState ReduceExpenseDeleted(ExpensesState state, ExpenseDeletedAction _)
    {
        return state with { ExpenseDeleting = true };
    }

    [ReducerMethod]
    public static ExpensesState ReduceExpenseDeletionSucceeded(ExpensesState state, ExpenseDeletionSucceededAction _)
    {
        return state with { ExpenseDeleting = false };
    }

    [ReducerMethod]
    public static ExpensesState ReduceExpenseDeletionFailed(ExpensesState state, ExpenseDeletionFailedAction action)
    {
        return state with { ExpenseDeleting = false };
    }

    [ReducerMethod]
    public static ExpensesState ReduceExpenseFiltersRequested(ExpensesState state, ExpenseFiltersRequestedAction _)
    {
        return state with { ExpenseFiltersRequesting = true };
    }

    [ReducerMethod]
    public static ExpensesState
        ReduceExpenseFiltersReceived(ExpensesState state, ExpenseFiltersReceivedAction action)
    {
        return state with { ExpenseFiltersRequesting = false, ExpenseFilters = action.Filters };
    }

    [ReducerMethod]
    public static ExpensesState ReduceExpenseFiltersRequestFailed(ExpensesState state,
        ExpenseFiltersRequestFailedAction action)
    {
        return state with { ExpenseFiltersRequesting = false };
    }
}

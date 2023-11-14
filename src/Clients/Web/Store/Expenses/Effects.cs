using Fluxor;
using Spenses.Client.Http;

namespace Spenses.Client.Web.Store.Expenses;

public class Effects
{
    private readonly IExpensesApi _expenses;

    public Effects(IExpensesApi expenses)
    {
        _expenses = expenses;
    }

    [EffectMethod]
    public async Task HandleExpensesRequested(ExpensesRequestedAction action, IDispatcher dispatcher)
    {
        var response = await _expenses.GetExpenses(action.HomeId, action.Query);

        if (response.Error is not null)
        {
            dispatcher.Dispatch(new ExpensesRequestFailedAction(response.Error));

            return;
        }

        dispatcher.Dispatch(new ExpensesReceivedAction(response.Content!));
    }

    [EffectMethod]
    public async Task HandleExpenseRequested(ExpenseRequestedAction action, IDispatcher dispatcher)
    {
        var response = await _expenses.GetExpense(action.HomeId, action.ExpenseId);

        if (response.Error is not null)
        {
            dispatcher.Dispatch(new ExpenseRequestFailedAction(response.Error));

            return;
        }

        dispatcher.Dispatch(new ExpenseReceivedAction(response.Content!));
    }

    [EffectMethod]
    public async Task HandleExpenseCreated(ExpenseCreatedAction action, IDispatcher dispatcher)
    {
        var response = await _expenses.PostExpense(action.HomeId, action.Props);

        if (response.Error is not null)
        {
            dispatcher.Dispatch(new ExpenseCreationFailedAction(response.Error));

            return;
        }

        dispatcher.Dispatch(new ExpenseCreationSucceededAction(response.Content!));
    }

    [EffectMethod]
    public async Task HandleExpenseUpdated(ExpenseUpdatedAction action, IDispatcher dispatcher)
    {
        var response = await _expenses.PutExpense(action.HomeId, action.ExpenseId, action.Props);

        if (response.Error is not null)
        {
            dispatcher.Dispatch(new ExpenseUpdateFailedAction(response.Error));

            return;
        }

        dispatcher.Dispatch(new ExpenseUpdateSucceededAction(response.Content!));
    }

    [EffectMethod]
    public async Task HandleExpenseDeleted(ExpenseDeletedAction action, IDispatcher dispatcher)
    {
        var response = await _expenses.DeleteExpense(action.HomeId, action.ExpenseId);

        if (response.Error is not null)
        {
            dispatcher.Dispatch(new ExpenseDeletionFailedAction(response.Error));

            return;
        }

        dispatcher.Dispatch(new ExpenseDeletionSucceededAction());
    }

    [EffectMethod]
    public async Task HandleExpenseFiltersRequested(ExpenseFiltersRequestedAction action, IDispatcher dispatcher)
    {
        var response = await _expenses.GetExpenseFilters(action.HomeId);

        if (response.Error is not null)
        {
            dispatcher.Dispatch(new ExpenseFiltersRequestFailedAction(response.Error));

            return;
        }

        dispatcher.Dispatch(new ExpenseFiltersReceivedAction(response.Content!));
    }
}

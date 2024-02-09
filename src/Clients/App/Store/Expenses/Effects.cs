using Fluxor;
using Spenses.App.Infrastructure;
using Spenses.App.Store.Shared;
using Spenses.Client.Http;

namespace Spenses.App.Store.Expenses;

public class Effects(IExpensesApi expenses)
{
    [EffectMethod]
    public async Task HandleExpenseRequested(ExpenseRequestedAction action, IDispatcher dispatcher)
    {
        var response = await expenses.GetExpense(action.HomeId, action.ExpenseId);

        if (response.Error is not null)
        {
            dispatcher.Dispatch(new ExpenseRequestFailedAction());
            dispatcher.Dispatch(new ApplicationErrorAction(response.Error.ToApplicationError()));

            return;
        }

        dispatcher.Dispatch(new ExpenseReceivedAction(response.Content!));
    }

    [EffectMethod]
    public async Task HandleExpenseCreated(ExpenseCreatedAction action, IDispatcher dispatcher)
    {
        var response = await expenses.PostExpense(action.HomeId, action.Props);

        if (response.Error is not null)
        {
            dispatcher.Dispatch(new ExpenseCreationFailedAction());
            dispatcher.Dispatch(new ApplicationErrorAction(response.Error.ToApplicationError()));

            return;
        }

        dispatcher.Dispatch(new ExpenseCreationSucceededAction(response.Content!));
    }

    [EffectMethod]
    public async Task HandleExpenseUpdated(ExpenseUpdatedAction action, IDispatcher dispatcher)
    {
        var response = await expenses.PutExpense(action.HomeId, action.ExpenseId, action.Props);

        if (response.Error is not null)
        {
            dispatcher.Dispatch(new ExpenseUpdateFailedAction());
            dispatcher.Dispatch(new ApplicationErrorAction(response.Error.ToApplicationError()));

            return;
        }

        dispatcher.Dispatch(new ExpenseUpdateSucceededAction(response.Content!));
    }

    [EffectMethod]
    public async Task HandleExpenseDeleted(ExpenseDeletedAction action, IDispatcher dispatcher)
    {
        var response = await expenses.DeleteExpense(action.HomeId, action.ExpenseId);

        if (response.Error is not null)
        {
            dispatcher.Dispatch(new ExpenseDeletionFailedAction());
            dispatcher.Dispatch(new ApplicationErrorAction(response.Error.ToApplicationError()));

            return;
        }

        dispatcher.Dispatch(new ExpenseDeletionSucceededAction());
    }

    [EffectMethod]
    public async Task HandleExpenseFiltersRequested(ExpenseFiltersRequestedAction action, IDispatcher dispatcher)
    {
        var response = await expenses.GetExpenseFilters(action.HomeId);

        if (response.Error is not null)
        {
            dispatcher.Dispatch(new ExpenseFiltersRequestFailedAction());
            dispatcher.Dispatch(new ApplicationErrorAction(response.Error.ToApplicationError()));

            return;
        }

        dispatcher.Dispatch(new ExpenseFiltersReceivedAction(response.Content!));
    }
}

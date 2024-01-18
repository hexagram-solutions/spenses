using Fluxor;
using Spenses.App.Infrastructure;
using Spenses.App.Store.Shared;
using Spenses.Client.Http;

namespace Spenses.App.Store.ExpenseCategories;

public class Effects(IExpenseCategoriesApi expenseCategories)
{
    [EffectMethod]
    public async Task HandleExpenseCategoriesRequested(ExpenseCategoriesRequestedAction action, IDispatcher dispatcher)
    {
        var response = await expenseCategories.GetExpenseCategories(action.HomeId);

        if (response.Error is not null)
        {
            dispatcher.Dispatch(new ExpenseCategoriesRequestFailedAction());
            dispatcher.Dispatch(new ApplicationErrorAction(response.Error.ToApplicationError()));


            return;
        }

        dispatcher.Dispatch(new ExpenseCategoriesReceivedAction(response.Content!.ToArray()));
    }

    [EffectMethod]
    public async Task HandleExpenseCategoryRequested(ExpenseCategoryRequestedAction action, IDispatcher dispatcher)
    {
        var response = await expenseCategories.GetExpenseCategory(action.HomeId, action.ExpenseCategoryId);

        if (response.Error is not null)
        {
            dispatcher.Dispatch(new ExpenseCategoryRequestFailedAction());
            dispatcher.Dispatch(new ApplicationErrorAction(response.Error.ToApplicationError()));

            return;
        }

        dispatcher.Dispatch(new ExpenseCategoryReceivedAction(response.Content!));
    }

    [EffectMethod]
    public async Task HandleExpenseCategoryCreated(ExpenseCategoryCreatedAction action, IDispatcher dispatcher)
    {
        var response = await expenseCategories.PostExpenseCategory(action.HomeId, action.Props);

        if (response.Error is not null)
        {
            dispatcher.Dispatch(new ExpenseCategoryCreationFailedAction());
            dispatcher.Dispatch(new ApplicationErrorAction(response.Error.ToApplicationError()));

            return;
        }

        dispatcher.Dispatch(new ExpenseCategoryCreationSucceededAction(response.Content!));
    }

    [EffectMethod]
    public async Task HandleExpenseCategoryUpdated(ExpenseCategoryUpdatedAction action, IDispatcher dispatcher)
    {
        var response = await expenseCategories.PutExpenseCategory(action.HomeId, action.ExpenseCategoryId, action.Props);

        if (response.Error is not null)
        {
            dispatcher.Dispatch(new ExpenseCategoryUpdateFailedAction());
            dispatcher.Dispatch(new ApplicationErrorAction(response.Error.ToApplicationError()));

            return;
        }

        dispatcher.Dispatch(new ExpenseCategoryUpdateSucceededAction(response.Content!));
    }

    [EffectMethod]
    public async Task HandleExpenseCategoryDeleted(ExpenseCategoryDeletedAction action, IDispatcher dispatcher)
    {
        var response = await expenseCategories.DeleteExpenseCategory(action.HomeId, action.ExpenseCategoryId);

        if (response.Error is not null)
        {
            dispatcher.Dispatch(new ExpenseCategoryDeletionFailedAction());
            dispatcher.Dispatch(new ApplicationErrorAction(response.Error.ToApplicationError()));

            return;
        }

        dispatcher.Dispatch(new ExpenseCategoryDeletionSucceededAction());
    }
}

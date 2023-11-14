using DynamicData;
using Fluxor;
using Spenses.Application.Models.ExpenseCategories;

namespace Spenses.Client.Web.Store.ExpenseCategories;

public static class Reducers
{
    [ReducerMethod]
    public static ExpenseCategoriesState ReduceExpenseCategoriesRequested(ExpenseCategoriesState state,
        ExpenseCategoriesRequestedAction _)
    {
        return state with { ExpenseCategoriesRequesting = true };
    }

    [ReducerMethod]
    public static ExpenseCategoriesState ReduceExpenseCategoriesReceived(ExpenseCategoriesState state,
        ExpenseCategoriesReceivedAction action)
    {
        return state with { ExpenseCategoriesRequesting = false, ExpenseCategories = action.ExpenseCategories };
    }

    [ReducerMethod]
    public static ExpenseCategoriesState ReduceExpenseCategoriesRequestFailed(ExpenseCategoriesState state,
        ExpenseCategoriesRequestFailedAction action)
    {
        return state with { ExpenseCategoriesRequesting = false, Error = action.Error };
    }

    [ReducerMethod]
    public static ExpenseCategoriesState ReduceExpenseCategoryRequested(ExpenseCategoriesState state,
        ExpenseCategoryRequestedAction _)
    {
        return state with { ExpenseCategoryRequesting = true };
    }

    [ReducerMethod]
    public static ExpenseCategoriesState ReduceExpenseCategoryReceived(ExpenseCategoriesState state,
        ExpenseCategoryReceivedAction action)
    {
        return state with { ExpenseCategoryRequesting = false, CurrentExpenseCategory = action.ExpenseCategory };
    }

    [ReducerMethod]
    public static ExpenseCategoriesState ReduceExpenseCategoryRequestFailed(ExpenseCategoriesState state,
        ExpenseCategoryRequestFailedAction action)
    {
        return state with { ExpenseCategoryRequesting = false, Error = action.Error };
    }

    [ReducerMethod]
    public static ExpenseCategoriesState ReduceExpenseCategoryCreated(ExpenseCategoriesState state,
        ExpenseCategoryCreatedAction _)
    {
        return state with { ExpenseCategoryCreating = true };
    }

    [ReducerMethod]
    public static ExpenseCategoriesState ReduceExpenseCategoryCreationSucceeded(ExpenseCategoriesState state,
        ExpenseCategoryCreationSucceededAction action)
    {
        var categories = new List<ExpenseCategory>(state.ExpenseCategories) { action.ExpenseCategory };

        return state with
        {
            ExpenseCategoryCreating = false,
            ExpenseCategories = categories
                .OrderByDescending(c => c.IsDefault)
                .ThenBy(c => c.Name)
                .ToArray()
        };
    }

    [ReducerMethod]
    public static ExpenseCategoriesState ReduceExpenseCategoryCreationFailed(ExpenseCategoriesState state,
        ExpenseCategoryCreationFailedAction action)
    {
        return state with { ExpenseCategoryCreating = false, Error = action.Error };
    }

    [ReducerMethod]
    public static ExpenseCategoriesState ReduceExpenseCategoryUpdated(ExpenseCategoriesState state,
        ExpenseCategoryUpdatedAction _)
    {
        return state with { ExpenseCategoryUpdating = true };
    }

    [ReducerMethod]
    public static ExpenseCategoriesState ReduceExpenseCategoryUpdateSucceeded(ExpenseCategoriesState state,
        ExpenseCategoryUpdateSucceededAction action)
    {
        var categories = new List<ExpenseCategory>(state.ExpenseCategories);

        var originalCategory = categories.Single(c => c.Id == action.ExpenseCategory.Id);

        categories.Replace(originalCategory, action.ExpenseCategory);

        return state with
        {
            ExpenseCategoryUpdating = false,
            CurrentExpenseCategory = action.ExpenseCategory,
            ExpenseCategories = categories.ToArray()
        };
    }

    [ReducerMethod]
    public static ExpenseCategoriesState ReduceExpenseCategoryUpdateFailed(ExpenseCategoriesState state,
        ExpenseCategoryUpdateFailedAction action)
    {
        return state with { ExpenseCategoryUpdating = false, Error = action.Error };
    }

    [ReducerMethod]
    public static ExpenseCategoriesState ReduceExpenseCategoryDeleted(ExpenseCategoriesState state,
        ExpenseCategoryDeletedAction _)
    {
        return state with { ExpenseCategoryDeleting = true };
    }

    [ReducerMethod]
    public static ExpenseCategoriesState ReduceExpenseCategoryDeletionSucceeded(ExpenseCategoriesState state,
        ExpenseCategoryDeletionSucceededAction _)
    {
        return state with { ExpenseCategoryDeleting = false };
    }

    [ReducerMethod]
    public static ExpenseCategoriesState ReduceExpenseCategoryDeletionFailed(ExpenseCategoriesState state,
        ExpenseCategoryDeletionFailedAction action)
    {
        return state with { ExpenseCategoryDeleting = false, Error = action.Error };
    }
}

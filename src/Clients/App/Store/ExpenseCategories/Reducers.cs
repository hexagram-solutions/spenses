using Fluxor;
using Hexagrams.Extensions.Common;
using Spenses.Shared.Models.ExpenseCategories;

namespace Spenses.App.Store.ExpenseCategories;

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
        ExpenseCategoriesRequestFailedAction _)
    {
        return state with { ExpenseCategoriesRequesting = false };
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
        ExpenseCategoryRequestFailedAction _)
    {
        return state with { ExpenseCategoryRequesting = false };
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
            ExpenseCategories =
            [
                .. categories
                    .OrderByDescending(c => c.IsDefault)
                    .ThenBy(c => c.Name),
            ]
        };
    }

    [ReducerMethod]
    public static ExpenseCategoriesState ReduceExpenseCategoryCreationFailed(ExpenseCategoriesState state,
        ExpenseCategoryCreationFailedAction _)
    {
        return state with { ExpenseCategoryCreating = false };
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
        var originalCategory = state.ExpenseCategories.Single(c => c.Id == action.ExpenseCategory.Id);

        return state with
        {
            ExpenseCategoryUpdating = false,
            CurrentExpenseCategory = action.ExpenseCategory,
            ExpenseCategories =
            [
                .. state.ExpenseCategories
                    .Replace(originalCategory, action.ExpenseCategory)
                    .OrderByDescending(ec => ec.IsDefault)
                    .ThenBy(ec => ec.Name),
            ]
        };
    }

    [ReducerMethod]
    public static ExpenseCategoriesState ReduceExpenseCategoryUpdateFailed(ExpenseCategoriesState state,
        ExpenseCategoryUpdateFailedAction _)
    {
        return state with { ExpenseCategoryUpdating = false };
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
        ExpenseCategoryDeletionFailedAction _)
    {
        return state with { ExpenseCategoryDeleting = false };
    }
}

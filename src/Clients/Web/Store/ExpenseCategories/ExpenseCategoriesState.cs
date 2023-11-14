using Fluxor;
using Refit;
using Spenses.Application.Models.ExpenseCategories;

namespace Spenses.Client.Web.Store.ExpenseCategories;

[FeatureState(Name = "ExpenseCategories", CreateInitialStateMethodName = nameof(Initialize))]
public record ExpenseCategoriesState
{
    private static ExpenseCategoriesState Initialize()
    {
        return new ExpenseCategoriesState();
    }

    public ExpenseCategory? CurrentExpenseCategory { get; init; }

    public ExpenseCategory[] ExpenseCategories { get; init; } = Array.Empty<ExpenseCategory>();

    public bool ExpenseCategoriesRequesting { get; init; }

    public bool ExpenseCategoryRequesting { get; init; }

    public bool ExpenseCategoryCreating { get; init; }

    public bool ExpenseCategoryUpdating { get; init; }

    public bool ExpenseCategoryDeleting { get; init; }

    public ApiException? Error { get; init; }
}

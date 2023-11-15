using Spenses.Application.Models.Common;
using Spenses.Application.Models.Expenses;
using Spenses.Client.Web.Infrastructure;

namespace Spenses.Client.Web.Store.Expenses;

public record ExpensesRequestedAction(Guid HomeId, FilteredExpensesQuery Query);

public record ExpensesReceivedAction(PagedResult<ExpenseDigest> Expenses);

public record ExpensesRequestFailedAction(ErrorMessage Error);

public record ExpenseRequestedAction(Guid HomeId, Guid ExpenseId);

public record ExpenseReceivedAction(Expense Expense);

public record ExpenseRequestFailedAction(ErrorMessage Error);

public record ExpenseCreatedAction(Guid HomeId, ExpenseProperties Props);

public record ExpenseCreationSucceededAction(Expense Expense);

public record ExpenseCreationFailedAction(ErrorMessage Error);

public record ExpenseUpdatedAction(Guid HomeId, Guid ExpenseId, ExpenseProperties Props);

public record ExpenseUpdateSucceededAction(Expense Expense);

public record ExpenseUpdateFailedAction(ErrorMessage Error);

public record ExpenseDeletedAction(Guid HomeId, Guid ExpenseId);

public record ExpenseDeletionSucceededAction;

public record ExpenseDeletionFailedAction(ErrorMessage Error);

public record ExpenseFiltersRequestedAction(Guid HomeId);

public record ExpenseFiltersReceivedAction(ExpenseFilters Filters);

public record ExpenseFiltersRequestFailedAction(ErrorMessage Error);

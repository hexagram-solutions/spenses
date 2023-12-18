using Spenses.Application.Models.Common;
using Spenses.Application.Models.Expenses;

namespace Spenses.Web.Client.Store.Expenses;

public record ExpensesRequestedAction(Guid HomeId, FilteredExpensesQuery Query);

public record ExpensesReceivedAction(PagedResult<ExpenseDigest> Expenses);

public record ExpensesRequestFailedAction;

public record ExpenseRequestedAction(Guid HomeId, Guid ExpenseId);

public record ExpenseReceivedAction(Expense Expense);

public record ExpenseRequestFailedAction;

public record ExpenseCreatedAction(Guid HomeId, ExpenseProperties Props);

public record ExpenseCreationSucceededAction(Expense Expense);

public record ExpenseCreationFailedAction;

public record ExpenseUpdatedAction(Guid HomeId, Guid ExpenseId, ExpenseProperties Props);

public record ExpenseUpdateSucceededAction(Expense Expense);

public record ExpenseUpdateFailedAction;

public record ExpenseDeletedAction(Guid HomeId, Guid ExpenseId);

public record ExpenseDeletionSucceededAction;

public record ExpenseDeletionFailedAction;

public record ExpenseFiltersRequestedAction(Guid HomeId);

public record ExpenseFiltersReceivedAction(ExpenseFilters Filters);

public record ExpenseFiltersRequestFailedAction;
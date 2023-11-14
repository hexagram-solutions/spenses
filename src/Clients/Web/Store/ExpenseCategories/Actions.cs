using Refit;
using Spenses.Application.Models.ExpenseCategories;
using Spenses.Application.Models.Expenses;

namespace Spenses.Client.Web.Store.ExpenseCategories;

public record ExpenseCategoriesRequestedAction(Guid HomeId);

public record ExpenseCategoriesReceivedAction(ExpenseCategory[] ExpenseCategories);

public record ExpenseCategoriesRequestFailedAction(ApiException Error);

public record ExpenseCategoryRequestedAction(Guid HomeId, Guid ExpenseCategoryId);

public record ExpenseCategoryReceivedAction(ExpenseCategory ExpenseCategory);

public record ExpenseCategoryRequestFailedAction(ApiException Error);

public record ExpenseCategoryCreatedAction(Guid HomeId, ExpenseCategoryProperties Props);

public record ExpenseCategoryCreationSucceededAction(ExpenseCategory ExpenseCategory);

public record ExpenseCategoryCreationFailedAction(ApiException Error);

public record ExpenseCategoryUpdatedAction(Guid HomeId, Guid ExpenseCategoryId, ExpenseCategoryProperties Props);

public record ExpenseCategoryUpdateSucceededAction(ExpenseCategory ExpenseCategory);

public record ExpenseCategoryUpdateFailedAction(ApiException Error);

public record ExpenseCategoryDeletedAction(Guid HomeId, Guid ExpenseCategoryId);

public record ExpenseCategoryDeletionSucceededAction;

public record ExpenseCategoryDeletionFailedAction(ApiException Error);

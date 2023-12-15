using Spenses.Application.Models.ExpenseCategories;

namespace Spenses.Web.Client.Store.ExpenseCategories;

public record ExpenseCategoriesRequestedAction(Guid HomeId);

public record ExpenseCategoriesReceivedAction(ExpenseCategory[] ExpenseCategories);

public record ExpenseCategoriesRequestFailedAction;

public record ExpenseCategoryRequestedAction(Guid HomeId, Guid ExpenseCategoryId);

public record ExpenseCategoryReceivedAction(ExpenseCategory ExpenseCategory);

public record ExpenseCategoryRequestFailedAction;

public record ExpenseCategoryCreatedAction(Guid HomeId, ExpenseCategoryProperties Props);

public record ExpenseCategoryCreationSucceededAction(ExpenseCategory ExpenseCategory);

public record ExpenseCategoryCreationFailedAction;

public record ExpenseCategoryUpdatedAction(Guid HomeId, Guid ExpenseCategoryId, ExpenseCategoryProperties Props);

public record ExpenseCategoryUpdateSucceededAction(ExpenseCategory ExpenseCategory);

public record ExpenseCategoryUpdateFailedAction;

public record ExpenseCategoryDeletedAction(Guid HomeId, Guid ExpenseCategoryId);

public record ExpenseCategoryDeletionSucceededAction;

public record ExpenseCategoryDeletionFailedAction;

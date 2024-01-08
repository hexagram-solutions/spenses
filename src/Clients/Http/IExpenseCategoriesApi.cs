using Refit;
using Spenses.Shared.Models.ExpenseCategories;

namespace Spenses.Client.Http;

public interface IExpenseCategoriesApi
{
    [Post("/homes/{homeId}/expense-categories")]
    Task<IApiResponse<ExpenseCategory>> PostExpenseCategory(Guid homeId, ExpenseCategoryProperties props);

    [Get("/homes/{homeId}/expense-categories")]
    Task<IApiResponse<IEnumerable<ExpenseCategory>>> GetExpenseCategories(Guid homeId);

    [Get("/homes/{homeId}/expense-categories/{expenseCategoryId}")]
    Task<IApiResponse<ExpenseCategory>> GetExpenseCategory(Guid homeId, Guid expenseCategoryId);

    [Put("/homes/{homeId}/expense-categories/{expenseCategoryId}")]
    Task<IApiResponse<ExpenseCategory>> PutExpenseCategory(Guid homeId, Guid expenseCategoryId,
        ExpenseCategoryProperties props);

    [Delete("/homes/{homeId}/expense-categories/{expenseCategoryId}")]
    Task<IApiResponse> DeleteExpenseCategory(Guid homeId, Guid expenseCategoryId);
}

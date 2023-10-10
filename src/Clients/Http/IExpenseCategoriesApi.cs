using Refit;
using Spenses.Application.Models.ExpenseCategories;

namespace Spenses.Client.Http;

public interface IExpenseCategoriesApi
{
    [Post("/homes/{homeId}/expense-categories")]
    Task<ApiResponse<ExpenseCategory>> PostExpenseCategory(Guid homeId, ExpenseCategoryProperties props);

    [Get("/homes/{homeId}/expense-categories")]
    Task<ApiResponse<IEnumerable<ExpenseCategory>>> GetExpenseCategories(Guid homeId);

    [Get("/homes/{homeId}/expense-categories/{expenseCategoryId}")]
    Task<ApiResponse<ExpenseCategory>> GetExpenseCategory(Guid homeId, Guid expenseCategoryId);

    [Put("/homes/{homeId}/expense-categories/{expenseCategoryId}")]
    Task<ApiResponse<ExpenseCategory>> PutExpenseCategory(Guid homeId, Guid expenseCategoryId,
        ExpenseCategoryProperties props);

    [Delete("/homes/{homeId}/expense-categories/{expenseCategoryId}")]
    Task DeleteExpenseCategory(Guid homeId, Guid expenseCategoryId);
}

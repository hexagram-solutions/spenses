using Refit;
using Spenses.Application.Models.Common;
using Spenses.Application.Models.Expenses;

namespace Spenses.Client.Http;

public interface IExpensesApi
{
    [Post("/homes/{homeId}/expenses")]
    Task<ApiResponse<Expense>> PostExpense(Guid homeId, ExpenseProperties props);

    [Get("/homes/{homeId}/expenses")]
    Task<ApiResponse<PagedResult<ExpenseDigest>>> GetExpenses(Guid homeId, [Query] FilteredExpensesQuery query);

    [Get("/homes/{homeId}/expenses/{expenseId}")]
    Task<ApiResponse<Expense>> GetExpense(Guid homeId, Guid expenseId);

    [Put("/homes/{homeId}/expenses/{expenseId}")]
    Task<ApiResponse<Expense>> PutExpense(Guid homeId, Guid expenseId, ExpenseProperties props);

    [Delete("/homes/{homeId}/expenses/{expenseId}")]
    Task DeleteExpense(Guid homeId, Guid expenseId);

    [Get("/homes/{homeId}/expenses/filters")]
    Task<ApiResponse<ExpenseFilters>> GetExpenseFilters(Guid homeId);
}

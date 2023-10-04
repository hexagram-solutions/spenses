using Refit;
using Spenses.Application.Models;

namespace Spenses.Client.Http;

public interface IHomeExpensesApi
{
    [Post("/homes/{homeId}/expenses")]
    Task<ApiResponse<Expense>> PostHomeExpense(Guid homeId, ExpenseProperties props);

    [Get("/homes/{homeId}/expenses")]
    Task<ApiResponse<PagedResult<ExpenseDigest>>> GetHomeExpenses(Guid homeId, [Query] FilteredExpensesQuery query);

    [Get("/homes/{homeId}/expenses/{expenseId}")]
    Task<ApiResponse<Expense>> GetHomeExpense(Guid homeId, Guid expenseId);

    [Put("/homes/{homeId}/expenses/{expenseId}")]
    Task<ApiResponse<Expense>> PutHomeExpense(Guid homeId, Guid expenseId, ExpenseProperties props);

    [Delete("/homes/{homeId}/expenses/{expenseId}")]
    Task DeleteHomeExpense(Guid homeId, Guid expenseId);

    [Get("/homes/{homeId}/expenses/filters")]
    Task<ApiResponse<ExpenseFilters>> GetExpenseFilters(Guid homeId);
}

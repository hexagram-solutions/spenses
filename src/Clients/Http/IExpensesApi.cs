using Refit;
using Spenses.Shared.Models.Common;
using Spenses.Shared.Models.Expenses;

namespace Spenses.Client.Http;

public interface IExpensesApi
{
    [Post("/homes/{homeId}/expenses")]
    Task<IApiResponse<Expense>> PostExpense(Guid homeId, ExpenseProperties props);

    [Get("/homes/{homeId}/expenses")]
    Task<IApiResponse<PagedResult<ExpenseDigest>>> GetExpenses(Guid homeId, [Query] FilteredExpensesQuery query);

    [Get("/homes/{homeId}/expenses/{expenseId}")]
    Task<IApiResponse<Expense>> GetExpense(Guid homeId, Guid expenseId);

    [Put("/homes/{homeId}/expenses/{expenseId}")]
    Task<IApiResponse<Expense>> PutExpense(Guid homeId, Guid expenseId, ExpenseProperties props);

    [Delete("/homes/{homeId}/expenses/{expenseId}")]
    Task<IApiResponse> DeleteExpense(Guid homeId, Guid expenseId);

    [Get("/homes/{homeId}/expenses/filters")]
    Task<IApiResponse<ExpenseFilters>> GetExpenseFilters(Guid homeId);
}

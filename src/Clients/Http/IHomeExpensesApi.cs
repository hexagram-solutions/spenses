using Refit;
using Spenses.Application.Models;

namespace Spenses.Client.Http;

public interface IHomeExpensesApi
{
    [Post("/homes/{homeId}/expenses")]
    Task<Expense> PostHomeExpense(Guid homeId, ExpenseProperties props);

    [Get("/homes/{homeId}/expenses")]
    Task<IEnumerable<Expense>> GetHomeExpenses(Guid homeId);

    [Get("/homes/{homeId}/expenses/{expenseId}")]
    Task<Expense> GetHomeExpense(Guid homeId, Guid expenseId);

    [Put("/homes/{homeId}/expenses/{expenseId}")]
    Task<Expense> PutHomeExpense(Guid homeId, Guid expenseId, ExpenseProperties props);

    [Delete("/homes/{homeId}/expenses/{expenseId}")]
    Task DeleteHomeExpense(Guid homeId, Guid expenseId);
}

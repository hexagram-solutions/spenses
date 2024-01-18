using Refit;
using Spenses.Shared.Models.Insights;

namespace Spenses.Client.Http;

public interface IInsightsApi
{
    [Get("/homes/{homeId}/insights/expenses-over-time")]
    Task<IApiResponse<IEnumerable<ExpenseTotalItem>>> GetExpensesOverTime(Guid homeId,
        [Query] ExpenseDateGrouping period);
}

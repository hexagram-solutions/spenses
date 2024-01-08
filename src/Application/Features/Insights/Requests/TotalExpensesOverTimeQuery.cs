using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Spenses.Application.Common.Behaviors;
using Spenses.Application.Features.Homes.Authorization;
using Spenses.Resources.Relational;
using Spenses.Shared.Models.Insights;

namespace Spenses.Application.Features.Insights.Requests;

public record TotalExpensesOverTimeQuery(Guid HomeId, ExpenseDateGrouping Grouping)
    : IAuthorizedRequest<IEnumerable<ExpenseTotalItem>>
{
    public AuthorizationPolicy Policy => Policies.MemberOfHomePolicy(HomeId);
}

public class TotalExpensesOverTimeQueryHandler(ApplicationDbContext db)
    : IRequestHandler<TotalExpensesOverTimeQuery, IEnumerable<ExpenseTotalItem>>
{
    public async Task<IEnumerable<ExpenseTotalItem>> Handle(TotalExpensesOverTimeQuery request,
        CancellationToken cancellationToken)
    {
        var (homeId, grouping) = request;

        var query = db.Expenses
            .Where(e => e.HomeId == homeId);

        var groupedQuery = grouping switch
        {
            ExpenseDateGrouping.Month => query.GroupBy(e => new DateOnly(e.Date.Year, e.Date.Month, 1)),
            ExpenseDateGrouping.Year => query.GroupBy(e => new DateOnly(e.Date.Year, 1, 1)),
            _ => throw new NotSupportedException($"Expense reporting period '{grouping}' is not supported"),
        };

        var values = await groupedQuery
            .Select(g => new { Date = g.Key, Value = g.Sum(s => s.Amount) })
            .ToDictionaryAsync(k => k.Date, v => v.Value, cancellationToken);

        return values.Select(v => new ExpenseTotalItem(v.Key, v.Value));
    }
}

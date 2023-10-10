using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Spenses.Application.Common.Behaviors;
using Spenses.Application.Features.Homes.Authorization;
using Spenses.Application.Models.Expenses;
using Spenses.Resources.Relational;

namespace Spenses.Application.Features.Expenses.Requests;

public record ExpenseFiltersQuery(Guid HomeId) : IAuthorizedRequest<ExpenseFilters>
{
    public AuthorizationPolicy Policy => Policies.MemberOfHomePolicy(HomeId);
}

public class ExpenseFiltersQueryHandler : IRequestHandler<ExpenseFiltersQuery, ExpenseFilters>
{
    private readonly ApplicationDbContext _db;

    public ExpenseFiltersQueryHandler(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<ExpenseFilters> Handle(ExpenseFiltersQuery request, CancellationToken cancellationToken)
    {
        var uniqueTags = await _db.Expenses
            .Where(e => e.HomeId == request.HomeId)
            .SelectMany(e => e.Tags.Select(t => t.Name))
            .Distinct()
            .ToArrayAsync(cancellationToken);

        var categories = await _db.ExpenseCategories
            .Where(e => e.HomeId == request.HomeId)
            .OrderBy(ec => ec.Name)
            .ToDictionaryAsync(k => k.Id, v => v.Name, cancellationToken);

        return new ExpenseFilters { Tags = uniqueTags.OrderBy(x => x).ToArray(), Categories = categories };
    }
}

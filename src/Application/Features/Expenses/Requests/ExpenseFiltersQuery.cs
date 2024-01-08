using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Spenses.Application.Common.Behaviors;
using Spenses.Application.Features.Homes.Authorization;
using Spenses.Resources.Relational;
using Spenses.Shared.Models.ExpenseCategories;
using Spenses.Shared.Models.Expenses;

namespace Spenses.Application.Features.Expenses.Requests;

public record ExpenseFiltersQuery(Guid HomeId) : IAuthorizedRequest<ExpenseFilters>
{
    public AuthorizationPolicy Policy => Policies.MemberOfHomePolicy(HomeId);
}

public class ExpenseFiltersQueryHandler(ApplicationDbContext db, IMapper mapper)
    : IRequestHandler<ExpenseFiltersQuery, ExpenseFilters>
{
    public async Task<ExpenseFilters> Handle(ExpenseFiltersQuery request, CancellationToken cancellationToken)
    {
        var uniqueTags = await db.Expenses
            .Where(e => e.HomeId == request.HomeId)
            .SelectMany(e => e.Tags.Select(t => t.Name))
            .Distinct()
            .ToArrayAsync(cancellationToken);

        var categories = await db.ExpenseCategories
            .Where(e => e.HomeId == request.HomeId)
            .OrderByDescending(ec => ec.IsDefault)
            .ThenBy(ec => ec.Name)
            .ProjectTo<ExpenseCategory>(mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken);

        return new ExpenseFilters { Tags = uniqueTags.OrderBy(x => x).ToArray(), Categories = categories };
    }
}

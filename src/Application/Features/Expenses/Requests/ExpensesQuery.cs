using AutoMapper;
using AutoMapper.QueryableExtensions;
using Hexagrams.Extensions.Common;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Spenses.Application.Common.Behaviors;
using Spenses.Application.Common.Query;
using Spenses.Application.Features.Homes.Authorization;
using Spenses.Application.Models.Common;
using Spenses.Application.Models.Expenses;
using Spenses.Resources.Relational;

namespace Spenses.Application.Features.Expenses.Requests;

public record ExpensesQuery(Guid HomeId) : FilteredExpensesQuery, IAuthorizedRequest<PagedResult<ExpenseDigest>>
{
    public AuthorizationPolicy Policy => Policies.MemberOfHomePolicy(HomeId);
}

public class ExpensesQueryHandler(ApplicationDbContext db, IMapper mapper)
    : IRequestHandler<ExpensesQuery, PagedResult<ExpenseDigest>>
{
    public async Task<PagedResult<ExpenseDigest>> Handle(ExpensesQuery request,
        CancellationToken cancellationToken)
    {
        var query = db.ExpenseDigests
            .Where(e => e.HomeId == request.HomeId)
            .ProjectTo<ExpenseDigest>(mapper.ConfigurationProvider);

        query = !string.IsNullOrEmpty(request.OrderBy) && request.SortDirection.HasValue
            ? query.OrderBy(new[] { request.OrderBy!.ToUpperCamelCase() }, request.SortDirection!.Value, true)
            : query.OrderBy(new[] { nameof(ExpenseDigest.Date) }, SortDirection.Desc, true);

        query = request.MinDate.HasValue
            ? query.Where(e => e.Date >= request.MinDate)
            : query;

        query = request.MaxDate.HasValue
            ? query.Where(e => e.Date <= request.MaxDate)
            : query;

        if (request.Tags?.Any() == true)
        {
            query = request.Tags
                .Aggregate(query, (current, tag) =>
                    current.Where(e => e.Tags != null && EF.Functions.Like(e.Tags, $"%{tag}%")));
        }

        if (request.Categories?.Any() == true)
            query = query.Where(e => e.CategoryId.HasValue && request.Categories.Contains(e.CategoryId.Value));

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip(request.Skip)
            .Take(request.Take)
            .ToListAsync(cancellationToken);

        return new PagedResult<ExpenseDigest>(totalCount, items);
    }
}

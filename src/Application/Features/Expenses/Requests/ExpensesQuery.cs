using AutoMapper;
using AutoMapper.QueryableExtensions;
using Hexagrams.Extensions.Common;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Spenses.Application.Behaviors;
using Spenses.Application.Features.Homes.Authorization;
using Spenses.Resources.Relational;
using Spenses.Shared.Common.Query;
using Spenses.Shared.Models.Common;
using Spenses.Shared.Models.Expenses;

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
            ? query.OrderBy([request.OrderBy!.ToUpperCamelCase()], request.SortDirection!.Value, true)
            : query.OrderBy([nameof(ExpenseDigest.Date)], SortDirection.Desc, true);

        query = query.Where(e => e.Date >= request.MinDate && e.Date <= request.MaxDate);

        if (request.Tags?.Any() == true)
        {
            query = request.Tags
                .Aggregate(query, (current, tag) =>
                    current.Where(e => e.Tags != null && EF.Functions.Like(e.Tags, $"%{tag}%")));
        }

        if (request.Categories?.Any() == true)
            query = query.Where(e => e.CategoryId.HasValue && request.Categories.Contains(e.CategoryId.Value));

        var totalCount = await query.CountAsync(cancellationToken);

        if (request.Skip.HasValue)
            query = query.Skip(request.Skip.Value);

        if (request.Take.HasValue)
            query = query.Take(request.Take.Value);

        var items = await query
            .ToListAsync(cancellationToken);

        return new PagedResult<ExpenseDigest>(totalCount, items);
    }
}

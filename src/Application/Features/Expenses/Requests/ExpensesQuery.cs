using AutoMapper;
using AutoMapper.QueryableExtensions;
using Hexagrams.Extensions.Common;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Spenses.Application.Common.Behaviors;
using Spenses.Application.Common.Query;
using Spenses.Application.Features.Homes.Authorization;
using Spenses.Application.Models;
using Spenses.Resources.Relational;

namespace Spenses.Application.Features.Expenses.Requests;

public record ExpensesQuery(Guid HomeId) : FilteredExpensesQuery, IAuthorizedRequest<PagedResult<ExpenseDigest>>
{
    public AuthorizationPolicy Policy => Policies.MemberOfHomePolicy(HomeId);
}

public class ExpensesQueryHandler : IRequestHandler<ExpensesQuery, PagedResult<ExpenseDigest>>
{
    private readonly ApplicationDbContext _db;
    private readonly IMapper _mapper;

    public ExpensesQueryHandler(ApplicationDbContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    public async Task<PagedResult<ExpenseDigest>> Handle(ExpensesQuery request,
        CancellationToken cancellationToken)
    {
        var query = _db.ExpenseDigests
            .Where(e => e.HomeId == request.HomeId)
            .ProjectTo<ExpenseDigest>(_mapper.ConfigurationProvider);

        query = !string.IsNullOrEmpty(request.OrderBy) && request.SortDirection.HasValue
            ? query.OrderBy(new[] { request.OrderBy!.ToUpperCamelCase() }, request.SortDirection!.Value, true)
            : query.OrderBy(new[] { nameof(ExpenseDigest.Date) }, SortDirection.Desc, true);

        query = request.MinDate.HasValue
            ? query.Where(e => e.Date >= request.MinDate)
            : query;

        query = request.MaxDate.HasValue
            ? query.Where(e => e.Date <= request.MaxDate)
            : query;

        query = request.Tags?.Any() == true
            ? query.Where(e => e.Tags != null &&
                e.Tags!.Split(' ', StringSplitOptions.None).Any(t => request.Tags.Contains(t)))
            : query;

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<ExpenseDigest>(totalCount, items);
    }
}

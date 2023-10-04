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
using Spenses.Application.Models.Credits;
using Spenses.Resources.Relational;

namespace Spenses.Application.Features.Credits.Requests;

public record CreditsQuery(Guid HomeId) : FilteredCreditsQuery, IAuthorizedRequest<PagedResult<CreditDigest>>
{
    public AuthorizationPolicy Policy => Policies.MemberOfHomePolicy(HomeId);
}

public class CreditsQueryHandler : IRequestHandler<CreditsQuery, PagedResult<CreditDigest>>
{
    private readonly ApplicationDbContext _db;
    private readonly IMapper _mapper;

    public CreditsQueryHandler(ApplicationDbContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    public async Task<PagedResult<CreditDigest>> Handle(CreditsQuery request,
        CancellationToken cancellationToken)
    {
        var query = _db.CreditDigests
            .Where(e => e.HomeId == request.HomeId)
            .ProjectTo<CreditDigest>(_mapper.ConfigurationProvider);

        query = !string.IsNullOrEmpty(request.OrderBy) && request.SortDirection.HasValue
            ? query.OrderBy(new[] { request.OrderBy!.ToUpperCamelCase() }, request.SortDirection!.Value, true)
            : query.OrderBy(new[] { nameof(CreditDigest.Date) }, SortDirection.Desc, true);

        query = request.MinDate.HasValue
            ? query.Where(e => e.Date >= request.MinDate)
            : query;

        query = request.MaxDate.HasValue
            ? query.Where(e => e.Date <= request.MaxDate)
            : query;

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<CreditDigest>(totalCount, items);
    }
}

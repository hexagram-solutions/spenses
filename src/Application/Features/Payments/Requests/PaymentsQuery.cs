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
using Spenses.Application.Models.Payments;
using Spenses.Resources.Relational;

namespace Spenses.Application.Features.Payments.Requests;

public record PaymentsQuery(Guid HomeId) : FilteredPaymentQuery, IAuthorizedRequest<PagedResult<PaymentDigest>>
{
    public AuthorizationPolicy Policy => Policies.MemberOfHomePolicy(HomeId);
}

public class PaymentsQueryHandler : IRequestHandler<PaymentsQuery, PagedResult<PaymentDigest>>
{
    private readonly ApplicationDbContext _db;
    private readonly IMapper _mapper;

    public PaymentsQueryHandler(ApplicationDbContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    public async Task<PagedResult<PaymentDigest>> Handle(PaymentsQuery request,
        CancellationToken cancellationToken)
    {
        var query = _db.PaymentDigests
            .Where(e => e.HomeId == request.HomeId)
            .ProjectTo<PaymentDigest>(_mapper.ConfigurationProvider);

        query = !string.IsNullOrEmpty(request.OrderBy) && request.SortDirection.HasValue
            ? query.OrderBy(new[] { request.OrderBy!.ToUpperCamelCase() }, request.SortDirection!.Value, true)
            : query.OrderBy(new[] { nameof(PaymentDigest.Date) }, SortDirection.Desc, true);

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

        return new PagedResult<PaymentDigest>(totalCount, items);
    }
}

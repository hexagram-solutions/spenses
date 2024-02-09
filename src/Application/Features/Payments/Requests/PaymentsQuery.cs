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
using Spenses.Shared.Models.Payments;

namespace Spenses.Application.Features.Payments.Requests;

public record PaymentsQuery(Guid HomeId) : FilteredPaymentsQuery, IAuthorizedRequest<PagedResult<PaymentDigest>>
{
    public AuthorizationPolicy Policy => Policies.MemberOfHomePolicy(HomeId);
}

public class PaymentsQueryHandler(ApplicationDbContext db, IMapper mapper)
    : IRequestHandler<PaymentsQuery, PagedResult<PaymentDigest>>
{
    public async Task<PagedResult<PaymentDigest>> Handle(PaymentsQuery request,
        CancellationToken cancellationToken)
    {
        var query = db.PaymentDigests
            .Where(e => e.HomeId == request.HomeId)
            .ProjectTo<PaymentDigest>(mapper.ConfigurationProvider);

        query = !string.IsNullOrEmpty(request.OrderBy) && request.SortDirection.HasValue
            ? query.OrderBy([request.OrderBy!.ToUpperCamelCase()], request.SortDirection!.Value, true)
            : query.OrderBy([nameof(PaymentDigest.Date)], SortDirection.Desc, true);

        query = query.Where(e => e.Date >= request.MinDate && e.Date <= request.MaxDate);

        var totalCount = await query.CountAsync(cancellationToken);

        if (request.Skip.HasValue)
            query = query.Skip(request.Skip.Value);

        if (request.Take.HasValue)
            query = query.Take(request.Take.Value);

        var items = await query
            .ToListAsync(cancellationToken);

        return new PagedResult<PaymentDigest>(totalCount, items);
    }
}

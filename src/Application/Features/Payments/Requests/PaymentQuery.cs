using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Spenses.Application.Behaviors;
using Spenses.Application.Exceptions;
using Spenses.Application.Features.Homes.Authorization;
using Spenses.Resources.Relational;
using Spenses.Shared.Models.Payments;

namespace Spenses.Application.Features.Payments.Requests;

public record PaymentQuery(Guid HomeId, Guid PaymentId) : IAuthorizedRequest<Payment>
{
    public AuthorizationPolicy Policy => Policies.MemberOfHomePolicy(HomeId);
}

public class PaymentQueryHandler(ApplicationDbContext db, IMapper mapper)
    : IRequestHandler<PaymentQuery, Payment>
{
    public async Task<Payment> Handle(PaymentQuery request, CancellationToken cancellationToken)
    {
        var (homeId, paymentId) = request;

        var payment = await db.Payments
            .Where(e => e.Home.Id == homeId)
            .ProjectTo<Payment>(mapper.ConfigurationProvider)
            .FirstOrDefaultAsync(h => h.Id == paymentId, cancellationToken);

        return payment ?? throw new ResourceNotFoundException(paymentId);
    }
}


using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Spenses.Application.Common.Behaviors;
using Spenses.Application.Exceptions;
using Spenses.Application.Features.Homes.Authorization;
using Spenses.Application.Models.Payments;
using Spenses.Resources.Relational;

namespace Spenses.Application.Features.Payments.Requests;

public record PaymentQuery(Guid HomeId, Guid PaymentId) : IAuthorizedRequest<Payment>
{
    public AuthorizationPolicy Policy => Policies.MemberOfHomePolicy(HomeId);
}

public class PaymentQueryHandler : IRequestHandler<PaymentQuery, Payment>
{
    private readonly ApplicationDbContext _db;
    private readonly IMapper _mapper;

    public PaymentQueryHandler(ApplicationDbContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    public async Task<Payment> Handle(PaymentQuery request, CancellationToken cancellationToken)
    {
        var (homeId, paymentId) = request;

        var payment = await _db.Payments
            .Where(e => e.Home.Id == homeId)
            .ProjectTo<Payment>(_mapper.ConfigurationProvider)
            .FirstOrDefaultAsync(h => h.Id == paymentId, cancellationToken);

        return payment ?? throw new ResourceNotFoundException(paymentId);
    }
}


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

public record UpdatePaymentCommand(Guid HomeId, Guid PaymentId, PaymentProperties Props)
    : IAuthorizedRequest<Payment>
{
    public AuthorizationPolicy Policy => Policies.MemberOfHomePolicy(HomeId);
}

public class UpdatePaymentCommandHandler : IRequestHandler<UpdatePaymentCommand, Payment>
{
    private readonly ApplicationDbContext _db;
    private readonly IMapper _mapper;

    public UpdatePaymentCommandHandler(ApplicationDbContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    public async Task<Payment> Handle(UpdatePaymentCommand request, CancellationToken cancellationToken)
    {
        var (homeId, paymentId, props) = request;

        var payment = await _db.Payments
            .Include(e => e.Home)
                .ThenInclude(h => h.Members)
            .Where(e => e.Home.Id == homeId)
            .FirstOrDefaultAsync(e => e.Id == paymentId, cancellationToken);

        if (payment is null)
            throw new ResourceNotFoundException(paymentId);

        if (payment.Home.Members.All(m => m.Id != props.PaidByMemberId))
            throw new InvalidRequestException($"Member {props.PaidByMemberId} is not a member of home {homeId}");

        if (payment.Home.Members.All(m => m.Id != props.PaidToMemberId))
            throw new InvalidRequestException($"Member {props.PaidToMemberId} is not a member of home {homeId}");

        _mapper.Map(request.Props, payment);

        payment.PaidByMemberId = props.PaidByMemberId;
        payment.PaidToMemberId = props.PaidToMemberId;

        await _db.SaveChangesAsync(cancellationToken);

        var updatedPayment = await _db.Payments
            .ProjectTo<Payment>(_mapper.ConfigurationProvider)
            .FirstAsync(e => e.Id == payment.Id, cancellationToken);

        return updatedPayment;
    }
}

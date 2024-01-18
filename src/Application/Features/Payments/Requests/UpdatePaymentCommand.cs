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

public record UpdatePaymentCommand(Guid HomeId, Guid PaymentId, PaymentProperties Props)
    : IAuthorizedRequest<Payment>
{
    public AuthorizationPolicy Policy => Policies.MemberOfHomePolicy(HomeId);
}

public class UpdatePaymentCommandHandler(ApplicationDbContext db, IMapper mapper)
    : IRequestHandler<UpdatePaymentCommand, Payment>
{
    public async Task<Payment> Handle(UpdatePaymentCommand request, CancellationToken cancellationToken)
    {
        var (homeId, paymentId, props) = request;

        var payment = await db.Payments
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

        mapper.Map(request.Props, payment);

        payment.PaidByMemberId = props.PaidByMemberId;
        payment.PaidToMemberId = props.PaidToMemberId;

        await db.SaveChangesAsync(cancellationToken);

        var updatedPayment = await db.Payments
            .ProjectTo<Payment>(mapper.ConfigurationProvider)
            .FirstAsync(e => e.Id == payment.Id, cancellationToken);

        return updatedPayment;
    }
}

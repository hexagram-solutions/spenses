using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Spenses.Application.Common.Behaviors;
using Spenses.Application.Exceptions;
using Spenses.Application.Features.Homes.Authorization;
using Spenses.Resources.Relational;

namespace Spenses.Application.Features.Payments.Requests;

public record DeletePaymentCommand(Guid HomeId, Guid PaymentId) : IAuthorizedRequest
{
    public AuthorizationPolicy Policy => Policies.MemberOfHomePolicy(HomeId);
}

public class DeletePaymentCommandHandler(ApplicationDbContext db)
    : IRequestHandler<DeletePaymentCommand>
{
    public async Task Handle(DeletePaymentCommand request, CancellationToken cancellationToken)
    {
        var (homeId, paymentId) = request;

        var payment = await db.Payments
            .Where(e => e.Home.Id == homeId)
            .FirstOrDefaultAsync(e => e.Id == paymentId, cancellationToken);

        if (payment is null)
            throw new ResourceNotFoundException(paymentId);

        db.Payments.Remove(payment);

        await db.SaveChangesAsync(cancellationToken);
    }
}

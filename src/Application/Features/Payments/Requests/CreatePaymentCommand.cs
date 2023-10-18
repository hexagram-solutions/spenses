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
using DbModels = Spenses.Resources.Relational.Models;

namespace Spenses.Application.Features.Payments.Requests;

public record CreatePaymentCommand(Guid HomeId, PaymentProperties Props) : IAuthorizedRequest<Payment>
{
    public AuthorizationPolicy Policy => Policies.MemberOfHomePolicy(HomeId);
}

public class CreatePaymentCommandHandler : IRequestHandler<CreatePaymentCommand, Payment>
{
    private readonly ApplicationDbContext _db;
    private readonly IMapper _mapper;

    public CreatePaymentCommandHandler(ApplicationDbContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    public async Task<Payment> Handle(CreatePaymentCommand request, CancellationToken cancellationToken)
    {
        var (homeId, props) = request;

        var home = await _db.Homes
            .Include(h => h.Members)
            .FirstOrDefaultAsync(h => h.Id == homeId, cancellationToken);

        if (home is null)
            throw new ResourceNotFoundException(homeId);

        if (home.Members.All(m => m.Id != props.PaidByMemberId))
            throw new InvalidRequestException($"Member {props.PaidByMemberId} is not a member of home {homeId}");

        var payment = _mapper.Map<DbModels.Payment>(props);

        payment.HomeId = homeId;
        payment.PaidByMemberId = props.PaidByMemberId;

        home.Payments.Add(payment);

        await _db.SaveChangesAsync(cancellationToken);

        var createdPayment = await _db.Payments
            .ProjectTo<Payment>(_mapper.ConfigurationProvider)
            .FirstAsync(e => e.Id == payment.Id, cancellationToken);

        return createdPayment;
    }
}

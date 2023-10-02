using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Spenses.Application.Common.Behaviors;
using Spenses.Application.Exceptions;
using Spenses.Application.Models;
using Spenses.Resources.Relational;

namespace Spenses.Application.Features.Homes.Credits;

public record UpdateCreditCommand(Guid HomeId, Guid CreditId, CreditProperties Props)
    : IAuthorizedRequest<Credit>
{
    public AuthorizationPolicy Policy => Policies.MemberOfHomePolicy(HomeId);
}

public class UpdateCreditCommandHandler : IRequestHandler<UpdateCreditCommand, Credit>
{
    private readonly ApplicationDbContext _db;
    private readonly IMapper _mapper;

    public UpdateCreditCommandHandler(ApplicationDbContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    public async Task<Credit> Handle(UpdateCreditCommand request, CancellationToken cancellationToken)
    {
        var (homeId, creditId, props) = request;

        var credit = await _db.Credits
            .Include(e => e.Home)
                .ThenInclude(h => h.Members)
            .Where(e => e.Home.Id == homeId)
            .FirstOrDefaultAsync(e => e.Id == creditId, cancellationToken);

        if (credit is null)
            throw new ResourceNotFoundException(creditId);

        if (credit.Home.Members.All(m => m.Id != props.PaidByMemberId))
            throw new InvalidRequestException($"Member {props.PaidByMemberId} is not a member of home {homeId}");

        _mapper.Map(request.Props, credit);

        credit.PaidByMemberId = props.PaidByMemberId;

        await _db.SaveChangesAsync(cancellationToken);

        var updatedCredit = await _db.Credits
            .ProjectTo<Credit>(_mapper.ConfigurationProvider)
            .FirstAsync(e => e.Id == credit.Id, cancellationToken);

        return updatedCredit;
    }
}

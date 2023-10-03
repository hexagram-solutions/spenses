using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Spenses.Application.Common.Behaviors;
using Spenses.Application.Exceptions;
using Spenses.Application.Features.Homes.Authorization;
using Spenses.Application.Models;
using Spenses.Resources.Relational;
using DbModels = Spenses.Resources.Relational.Models;

namespace Spenses.Application.Features.Credits.Requests;

public record CreateCreditCommand(Guid HomeId, CreditProperties Props) : IAuthorizedRequest<Credit>
{
    public AuthorizationPolicy Policy => Policies.MemberOfHomePolicy(HomeId);
}

public class CreateCreditCommandHandler : IRequestHandler<CreateCreditCommand, Credit>
{
    private readonly ApplicationDbContext _db;
    private readonly IMapper _mapper;

    public CreateCreditCommandHandler(ApplicationDbContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    public async Task<Credit> Handle(CreateCreditCommand request, CancellationToken cancellationToken)
    {
        var (homeId, props) = request;

        var home = await _db.Homes
            .Include(h => h.Members)
            .FirstOrDefaultAsync(h => h.Id == homeId, cancellationToken);

        if (home is null)
            throw new ResourceNotFoundException(homeId);

        if (home.Members.All(m => m.Id != props.PaidByMemberId))
            throw new InvalidRequestException($"Member {props.PaidByMemberId} is not a member of home {homeId}");

        var credit = _mapper.Map<DbModels.Credit>(props);

        credit.HomeId = homeId;
        credit.PaidByMemberId = props.PaidByMemberId;

        home.Credits.Add(credit);

        await _db.SaveChangesAsync(cancellationToken);

        var createdCredit = await _db.Credits
            .ProjectTo<Credit>(_mapper.ConfigurationProvider)
            .FirstAsync(e => e.Id == credit.Id, cancellationToken);

        return createdCredit;
    }
}

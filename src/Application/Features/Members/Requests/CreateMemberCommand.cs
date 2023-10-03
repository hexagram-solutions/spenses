using AutoMapper;
using FluentValidation.Results;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Spenses.Application.Common.Behaviors;
using Spenses.Application.Exceptions;
using Spenses.Application.Features.Homes.Authorization;
using Spenses.Application.Models;
using Spenses.Resources.Relational;
using DbModels = Spenses.Resources.Relational.Models;

namespace Spenses.Application.Features.Members.Requests;

public record CreateMemberCommand(Guid HomeId, MemberProperties Props) : IAuthorizedRequest<Member>
{
    public AuthorizationPolicy Policy => Policies.MemberOfHomePolicy(HomeId);
}

public class CreateMemberCommandHandler : IRequestHandler<CreateMemberCommand, Member>
{
    private readonly ApplicationDbContext _db;
    private readonly IMapper _mapper;

    public CreateMemberCommandHandler(ApplicationDbContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    public async Task<Member> Handle(CreateMemberCommand request, CancellationToken cancellationToken)
    {
        var (homeId, props) = request;

        var home = await _db.Homes
            .Include(h => h.Members)
            .FirstOrDefaultAsync(h => h.Id == homeId, cancellationToken);

        if (home == null)
            throw new ResourceNotFoundException(homeId);

        var otherMembersSplitPercentageTotal = home.Members.Sum(m => m.SplitPercentage);

        if (otherMembersSplitPercentageTotal + props.SplitPercentage > 1)
        {
            throw new InvalidRequestException(
                new ValidationFailure(nameof(MemberProperties.SplitPercentage),
                    "Total split percentage among home members cannot exceed 100%"));
        }

        var member = _mapper.Map<DbModels.Member>(props);

        home.Members.Add(member);

        await _db.SaveChangesAsync(cancellationToken);

        return _mapper.Map<Member>(_db.Entry(member).Entity);
    }
}

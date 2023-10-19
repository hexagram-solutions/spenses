using AutoMapper;
using FluentValidation.Results;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Spenses.Application.Common.Behaviors;
using Spenses.Application.Exceptions;
using Spenses.Application.Features.Homes.Authorization;
using Spenses.Application.Models.Members;
using Spenses.Resources.Relational;

namespace Spenses.Application.Features.Members.Requests;

public record UpdateMemberCommand(Guid HomeId, Guid MemberId, MemberProperties Props)
    : IAuthorizedRequest<Member>
{
    public AuthorizationPolicy Policy => Policies.MemberOfHomePolicy(HomeId);
}

public class UpdateMemberCommandHandler : IRequestHandler<UpdateMemberCommand, Member>
{
    private readonly ApplicationDbContext _db;
    private readonly IMapper _mapper;

    public UpdateMemberCommandHandler(ApplicationDbContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    public async Task<Member> Handle(UpdateMemberCommand request, CancellationToken cancellationToken)
    {
        var (homeId, memberId, props) = request;

        var homeMembers = await _db.Members
            .Where(m => m.HomeId == homeId)
            .ToListAsync(cancellationToken);

        var memberToUpdate = homeMembers
            .FirstOrDefault(h => h.Id == memberId) ?? throw new ResourceNotFoundException(memberId);

        var otherMembersSplitPercentageTotal = homeMembers
            .Where(m => m.Id != memberId)
            .Sum(m => m.DefaultSplitPercentage);

        if (otherMembersSplitPercentageTotal + props.DefaultSplitPercentage > 1)
        {
            throw new InvalidRequestException(
                new ValidationFailure(nameof(MemberProperties.DefaultSplitPercentage),
                    "Total split percentage among home members cannot exceed 100%"));
        }

        _mapper.Map(props, memberToUpdate);

        await _db.SaveChangesAsync(cancellationToken);

        return _mapper.Map<Member>(memberToUpdate);
    }
}

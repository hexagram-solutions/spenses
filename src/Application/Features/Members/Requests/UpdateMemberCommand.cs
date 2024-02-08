using AutoMapper;
using FluentValidation.Results;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Spenses.Application.Behaviors;
using Spenses.Application.Exceptions;
using Spenses.Application.Features.Homes.Authorization;
using Spenses.Resources.Relational;
using Spenses.Shared.Models.Members;

namespace Spenses.Application.Features.Members.Requests;

public record UpdateMemberCommand(Guid HomeId, Guid MemberId, MemberProperties Props)
    : IAuthorizedRequest<Member>
{
    public AuthorizationPolicy Policy => Policies.MemberOfHomePolicy(HomeId);
}

public class UpdateMemberCommandHandler(ApplicationDbContext db, IMapper mapper)
    : IRequestHandler<UpdateMemberCommand, Member>
{
    public async Task<Member> Handle(UpdateMemberCommand request, CancellationToken cancellationToken)
    {
        var (homeId, memberId, props) = request;

        var homeMembers = await db.Members
            .Where(m => m.HomeId == homeId)
            .ToListAsync(cancellationToken);

        var memberToUpdate = homeMembers
            .FirstOrDefault(h => h.Id == memberId) ?? throw new ResourceNotFoundException(memberId);

        var otherHomeMembers = homeMembers
            .Where(m => m.Id != memberId)
            .ToList();

        var duplicateContactEmail = !string.IsNullOrEmpty(props.ContactEmail) &&
            otherHomeMembers.Any(m => string.Equals(m.ContactEmail?.ToLower(), props.ContactEmail.ToLower(),
                StringComparison.OrdinalIgnoreCase));

        if (duplicateContactEmail)
        {
            throw new InvalidRequestException(new ValidationFailure(nameof(MemberProperties.ContactEmail),
                $"A member with the contact email \"{props.ContactEmail}\" already exists in this home."));
        }

        var otherMembersSplitPercentageTotal = otherHomeMembers.Sum(m => m.DefaultSplitPercentage);

        if (otherMembersSplitPercentageTotal + props.DefaultSplitPercentage > 1)
        {
            throw new InvalidRequestException(
                new ValidationFailure(nameof(MemberProperties.DefaultSplitPercentage),
                    "Total split percentage among home members cannot exceed 100%"));
        }

        mapper.Map(props, memberToUpdate);

        await db.SaveChangesAsync(cancellationToken);

        return mapper.Map<Member>(memberToUpdate);
    }
}

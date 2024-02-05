using AutoMapper;
using FluentValidation.Results;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Spenses.Application.Behaviors;
using Spenses.Application.Exceptions;
using Spenses.Application.Features.Homes.Authorization;
using Spenses.Resources.Relational;
using Spenses.Shared.Models.Invitations;
using Spenses.Shared.Models.Members;
using DbModels = Spenses.Resources.Relational.Models;

namespace Spenses.Application.Features.Members.Requests;

public record CreateMemberCommand(Guid HomeId, CreateMemberProperties Props) : IAuthorizedRequest<Member>
{
    public AuthorizationPolicy Policy => Policies.MemberOfHomePolicy(HomeId);
}

public class CreateMemberCommandHandler(ApplicationDbContext db, IMapper mapper, ISender sender)
    : IRequestHandler<CreateMemberCommand, Member>
{
    public async Task<Member> Handle(CreateMemberCommand request, CancellationToken cancellationToken)
    {
        var (homeId, props) = request;

        var home = await db.Homes
            .Include(h => h.Members)
            .FirstAsync(h => h.Id == homeId, cancellationToken);

        var otherMembersSplitPercentageTotal = home.Members.Sum(m => m.DefaultSplitPercentage);

        if (otherMembersSplitPercentageTotal + props.DefaultSplitPercentage > 1)
        {
            throw new InvalidRequestException(
                new ValidationFailure(nameof(MemberProperties.DefaultSplitPercentage),
                    "Total split percentage among home members cannot exceed 100%"));
        }

        var member = mapper.Map<DbModels.Member>(props);

        member.Status = DbModels.MemberStatus.Active;

        home.Members.Add(member);

        await db.SaveChangesAsync(cancellationToken);

        if (props.ShouldInvite)
        {
            await sender.Send(new InviteExistingMemberCommand(home.Id, member.Id,
                new InvitationProperties { Email = member.ContactEmail! }), cancellationToken);
        }

        return mapper.Map<Member>(member);
    }
}

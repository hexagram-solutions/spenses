using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Spenses.Application.Behaviors;
using Spenses.Application.Features.Homes.Authorization;
using Spenses.Resources.Relational;
using Spenses.Shared.Models.Invitations;

namespace Spenses.Application.Features.Invitations.Requests;

public record CreateInvitationCommand(Guid HomeId, InvitationProperties Props) : IAuthorizedRequest<Invitation>
{
    public AuthorizationPolicy Policy => Policies.MemberOfHomePolicy(HomeId);
}

public class CreateInvitationCommandHandler(ApplicationDbContext db, IMapper mapper)
    : IRequestHandler<CreateInvitationCommand, Invitation>
{
    public Task<Invitation> Handle(CreateInvitationCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}

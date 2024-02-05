using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Spenses.Application.Exceptions;
using Spenses.Application.Services.Invitations;
using Spenses.Resources.Relational;
using Spenses.Shared.Models.Invitations;

namespace Spenses.Application.Features.Invitations.Requests;

public record InvitationQuery(string Token) : IRequest<Invitation>;

public class InvitationQueryHandler(
    ApplicationDbContext db,
    InvitationTokenProvider invitationTokenProvider,
    IMapper mapper)
    : IRequestHandler<InvitationQuery, Invitation>
{
    public async Task<Invitation> Handle(InvitationQuery request, CancellationToken cancellationToken)
    {
        var token = request.Token;

        if (!invitationTokenProvider.TryUnprotectInvitationData(token, out var invitationData))
            throw new ResourceNotFoundException(token);

        var invitation = await db.Invitations
            .ProjectTo<Invitation>(mapper.ConfigurationProvider)
            .FirstOrDefaultAsync(i => i.Id == invitationData!.InvitationId, cancellationToken);

        if (invitation is not { Status: InvitationStatus.Pending })
            throw new ResourceNotFoundException(token);

        return invitation;
    }
}

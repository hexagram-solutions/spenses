using MediatR;
using Spenses.Shared.Models.Invitations;

namespace Spenses.Application.Features.Invitations.Requests;

public record AcceptInvitationCommand(Guid HomeId, Guid InvitationId) : IRequest<Invitation>;
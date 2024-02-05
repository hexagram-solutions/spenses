using Spenses.Shared.Models.Invitations;

namespace Spenses.App.Store.Invitations;

public record InvitationRequestedAction(string Token);

public record InvitationReceivedAction(Invitation Invitation);

public record InvitationRequestFailedAction;

public record InvitationAcceptedAction(Guid InvitationId);

public record InvitationAcceptanceSucceededAction;

public record InvitationAcceptanceFailedAction;

public record InvitationDeclinedAction(Guid InvitationId);

public record InvitationDeclinationSucceededAction;

public record InvitationDeclinationFailedAction;

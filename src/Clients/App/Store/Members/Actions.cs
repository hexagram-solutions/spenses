using Spenses.Shared.Models.Common;
using Spenses.Shared.Models.Invitations;
using Spenses.Shared.Models.Members;

namespace Spenses.App.Store.Members;

public record MembersRequestedAction(Guid HomeId);

public record MembersReceivedAction(IEnumerable<Member> Members);

public record MembersRequestFailedAction;

public record MemberRequestedAction(Guid HomeId, Guid MemberId);

public record MemberReceivedAction(Member Member);

public record MemberRequestFailedAction;

public record MemberCreatedAction(Guid HomeId, CreateMemberProperties Props);

public record MemberCreationSucceededAction(Member Member);

public record MemberCreationFailedAction(Dictionary<string, string[]> Errors);

public record MemberUpdatedAction(Guid HomeId, Guid MemberId, MemberProperties Props);

public record MemberUpdateSucceededAction(Member Member);

public record MemberUpdateFailedAction(Dictionary<string, string[]> Errors);

public record MemberDeletedAction(Guid HomeId, Guid MemberId);

public record MemberDeletionSucceededAction(DeletionResult<Member> Result);

public record MemberDeletionFailedAction;

public record MemberActivatedAction(Guid HomeId, Guid MemberId);

public record MemberActivationSucceededAction(Member Member);

public record MemberActivationFailedAction;

public record MemberInvitedAction(Guid HomeId, Guid MemberId, InvitationProperties Props);

public record MemberInvitationSucceededAction(Invitation Invitation);

public record MemberInvitationFailedAction;

public record MemberInvitationsCancelledAction(Guid HomeId, Guid MemberId);

public record MemberInvitationsCancellationSucceededAction(Guid MemberId);

public record MemberInvitationsCancellationFailedAction;

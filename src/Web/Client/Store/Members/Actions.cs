using Spenses.Shared.Models.Common;
using Spenses.Shared.Models.Members;

namespace Spenses.Web.Client.Store.Members;

public record MembersRequestedAction(Guid HomeId);

public record MembersReceivedAction(IEnumerable<Member> Members);

public record MembersRequestFailedAction;

public record MemberRequestedAction(Guid HomeId, Guid MemberId);

public record MemberReceivedAction(Member Member);

public record MemberRequestFailedAction;

public record MemberCreatedAction(Guid HomeId, MemberProperties Props);

public record MemberCreationSucceededAction(Member Member);

public record MemberCreationFailedAction;

public record MemberUpdatedAction(Guid HomeId, Guid MemberId, MemberProperties Props);

public record MemberUpdateSucceededAction(Member Member);

public record MemberUpdateFailedAction;

public record MemberDeletedAction(Guid HomeId, Guid MemberId);

public record MemberDeletionSucceededAction(DeletionResult<Member> Result);

public record MemberDeletionFailedAction;

public record MemberActivatedAction(Guid HomeId, Guid MemberId);

public record MemberActivationSucceededAction(Member Member);

public record MemberActivationFiledAction;

using Refit;
using Spenses.Application.Models.Common;
using Spenses.Application.Models.Members;

namespace Spenses.Client.Web.Store.Members;

public record MembersRequestedAction(Guid HomeId);

public record MembersReceivedAction(IEnumerable<Member> Members);

public record MembersRequestFailedAction(ApiException Error);

public record MemberRequestedAction(Guid HomeId, Guid MemberId);

public record MemberReceivedAction(Member Member);

public record MemberRequestFailedAction(ApiException Error);

public record MemberCreatedAction(Guid HomeId, MemberProperties Props);

public record MemberCreationSucceededAction(Member Member);

public record MemberCreationFailedAction(ApiException Error);

public record MemberUpdatedAction(Guid HomeId, Guid MemberId, MemberProperties Props);

public record MemberUpdateSucceededAction(Member Member);

public record MemberUpdateFailedAction(ApiException Error);

public record MemberDeletedAction(Guid HomeId, Guid MemberId);

public record MemberDeletionSucceededAction(DeletionResult<Member> Result);

public record MemberDeletionFailedAction(ApiException Error);

public record MemberActivatedAction(Guid HomeId, Guid MemberId);

public record MemberActivationSucceededAction(Member Member);

public record MemberActivationFiledAction(ApiException Error);

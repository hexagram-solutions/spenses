using Fluxor;
using Hexagrams.Extensions.Common;
using Spenses.Shared.Models.Members;

namespace Spenses.App.Store.Members;

public static class Reducers
{
    [ReducerMethod]
    public static MembersState ReduceMembersRequested(MembersState state, MembersRequestedAction _)
    {
        return state with { MembersRequesting = true };
    }

    [ReducerMethod]
    public static MembersState ReduceMembersReceived(MembersState state, MembersReceivedAction action)
    {
        return state with { MembersRequesting = false, Members = action.Members.ToArray() };
    }

    [ReducerMethod]
    public static MembersState ReduceMembersRequestFailed(MembersState state, MembersRequestFailedAction _)
    {
        return state with { MembersRequesting = false };
    }

    [ReducerMethod]
    public static MembersState ReduceMemberRequested(MembersState state, MemberRequestedAction _)
    {
        return state with { MemberRequesting = true };
    }

    [ReducerMethod]
    public static MembersState ReduceMemberReceived(MembersState state, MemberReceivedAction action)
    {
        return state with { MemberRequesting = false, CurrentMember = action.Member };
    }

    [ReducerMethod]
    public static MembersState ReduceMemberRequestedFailed(MembersState state, MemberRequestFailedAction _)
    {
        return state with { MemberRequesting = false };
    }

    [ReducerMethod]
    public static MembersState ReduceMemberCreated(MembersState state, MemberCreatedAction _)
    {
        return state with { MemberCreating = true, Errors = [] };
    }

    [ReducerMethod]
    public static MembersState ReduceMemberCreationSucceeded(MembersState state,
        MemberCreationSucceededAction action)
    {
        return state with { MemberCreating = false, CurrentMember = action.Member, Errors = [] };
    }

    [ReducerMethod]
    public static MembersState ReduceMemberCreationFailed(MembersState state, MemberCreationFailedAction action)
    {
        return state with { MemberCreating = false, Errors = action.Errors };
    }

    [ReducerMethod]
    public static MembersState ReduceMemberUpdated(MembersState state, MemberUpdatedAction _)
    {
        return state with { MemberUpdating = true, Errors = [] };
    }

    [ReducerMethod]
    public static MembersState ReduceMemberUpdateSucceeded(MembersState state, MemberUpdateSucceededAction action)
    {
        return state with { MemberUpdating = false, CurrentMember = action.Member, Errors = [] };
    }

    [ReducerMethod]
    public static MembersState ReduceMemberUpdateFailed(MembersState state, MemberUpdateFailedAction action)
    {
        return state with { MemberUpdating = false, Errors = action.Errors };
    }

    [ReducerMethod]
    public static MembersState ReduceMemberDeleted(MembersState state, MemberDeletedAction _)
    {
        return state with { MemberDeleting = true };
    }

    [ReducerMethod]
    public static MembersState ReduceMemberDeleteSucceeded(MembersState state, MemberDeletionSucceededAction _)
    {
        return state with { MemberDeleting = false };
    }

    [ReducerMethod]
    public static MembersState ReduceMemberDeleteFailed(MembersState state, MemberDeletionFailedAction _)
    {
        return state with { MemberDeleting = false };
    }

    [ReducerMethod]
    public static MembersState ReduceMemberActivated(MembersState state, MemberActivatedAction _)
    {
        return state with { MemberActivating = true };
    }

    [ReducerMethod]
    public static MembersState ReduceMemberActivationSucceeded(MembersState state, MemberActivationSucceededAction _)
    {
        return state with { MemberActivating = false };
    }

    [ReducerMethod]
    public static MembersState ReduceMemberActivationFailed(MembersState state, MemberActivationFailedAction _)
    {
        return state with { MemberActivating = false };
    }

    [ReducerMethod]
    public static MembersState ReduceMemberInvited(MembersState state, MemberInvitedAction _)
    {
        return state with { MemberInviting = true };
    }

    [ReducerMethod]
    public static MembersState ReduceMemberInvitationSucceeded(MembersState state,
        MemberInvitationSucceededAction action)
    {
        var invitedMember = state.Members.Single(m => m.Id == action.Invitation.Member.Id);

        return state with
        {
            MemberInviting = false,
            Members = state.Members.Replace(invitedMember, action.Invitation.Member).ToArray()
        };
    }

    [ReducerMethod]
    public static MembersState ReduceMemberInvitationFailed(MembersState state, MemberInvitationFailedAction _)
    {
        return state with { MemberInviting = false };
    }

    [ReducerMethod]
    public static MembersState ReduceMemberInvitationsCancelled(MembersState state, MemberInvitationsCancelledAction _)
    {
        return state with { MemberInvitationsCancelling = true };
    }

    [ReducerMethod]
    public static MembersState ReduceMemberInvitationsCancellationSucceeded(MembersState state,
        MemberInvitationsCancellationSucceededAction action)
    {
        var member = state.Members.Single(m => m.Id == action.MemberId);

        return state with
        {
            MemberInvitationsCancelling = false,
            Members = state.Members.Replace(member, member with { Status = MemberStatus.Active }).ToArray()
        };
    }

    [ReducerMethod]
    public static MembersState ReduceMemberInvitationsCancellationFailed(MembersState state,
        MemberInvitationsCancellationFailedAction _)
    {
        return state with { MemberInvitationsCancelling = false };
    }
}

using Fluxor;

namespace Spenses.Client.Web.Store.Members;

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
    public static MembersState ReduceMembersRequestFailed(MembersState state, MembersRequestFailedAction action)
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
    public static MembersState ReduceMemberRequestedFailed(MembersState state, MemberRequestFailedAction action)
    {
        return state with { MemberRequesting = false };
    }

    [ReducerMethod]
    public static MembersState ReduceMemberCreated(MembersState state, MemberCreatedAction _)
    {
        return state with { MemberCreating = true };
    }

    [ReducerMethod]
    public static MembersState ReduceMemberCreationSucceeded(MembersState state,
        MemberCreationSucceededAction action)
    {
        return state with { MemberCreating = false, CurrentMember = action.Member };
    }

    [ReducerMethod]
    public static MembersState ReduceMemberCreationFailed(MembersState state, MemberCreationFailedAction action)
    {
        return state with { MemberCreating = false };
    }

    [ReducerMethod]
    public static MembersState ReduceMemberUpdated(MembersState state, MemberUpdatedAction _)
    {
        return state with { MemberUpdating = true };
    }

    [ReducerMethod]
    public static MembersState ReduceMemberUpdateSucceeded(MembersState state, MemberUpdateSucceededAction action)
    {
        return state with { MemberUpdating = false, CurrentMember = action.Member };
    }

    [ReducerMethod]
    public static MembersState ReduceMemberUpdateFailed(MembersState state, MemberUpdateFailedAction action)
    {
        return state with { MemberUpdating = false };
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
    public static MembersState ReduceMemberDeleteFailed(MembersState state, MemberDeletionFailedAction action)
    {
        return state with { MemberDeleting = false };
    }

    [ReducerMethod]
    public static MembersState ReduceMemberActivated(MembersState state, MemberActivatedAction _)
    {
        return state with { MemberActivating = true };
    }

    [ReducerMethod]
    public static MembersState ReduceMemberActivationSucceeded(MembersState state, MemberDeletionSucceededAction _)
    {
        return state with { MemberActivating = false };
    }

    [ReducerMethod]
    public static MembersState ReduceMemberActivationFailed(MembersState state, MemberDeletionFailedAction action)
    {
        return state with { MemberActivating = false };
    }
}

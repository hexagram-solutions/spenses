using Fluxor;
using Spenses.Client.Http;

namespace Spenses.Client.Web.Store.Members;

public class Effects
{
    private readonly IMembersApi _members;

    public Effects(IMembersApi members)
    {
        _members = members;
    }

    [EffectMethod]
    public async Task HandleMembersRequested(MembersRequestedAction action, IDispatcher dispatcher)
    {
        var response = await _members.GetMembers(action.HomeId);

        if (response.Error is not null)
        {
            dispatcher.Dispatch(new MembersRequestFailedAction(response.Error));

            return;
        }

        dispatcher.Dispatch(new MembersReceivedAction(response.Content!));
    }

    [EffectMethod]
    public async Task HandleMemberRequested(MemberRequestedAction action, IDispatcher dispatcher)
    {
        var response = await _members.GetMember(action.HomeId, action.MemberId);

        if (response.Error is not null)
        {
            dispatcher.Dispatch(new MemberRequestFailedAction(response.Error));

            return;
        }

        dispatcher.Dispatch(new MemberReceivedAction(response.Content!));
    }

    [EffectMethod]
    public async Task HandleMemberCreated(MemberCreatedAction action, IDispatcher dispatcher)
    {
        var response = await _members.PostMember(action.HomeId, action.Props);

        if (response.Error is not null)
        {
            dispatcher.Dispatch(new MemberCreationFailedAction(response.Error));

            return;
        }

        dispatcher.Dispatch(new MemberCreationSucceededAction(response.Content!));
        dispatcher.Dispatch(new MembersRequestedAction(action.HomeId));
    }

    [EffectMethod]
    public async Task HandleMemberUpdated(MemberUpdatedAction action, IDispatcher dispatcher)
    {
        var response = await _members.PutMember(action.HomeId, action.MemberId, action.Props);

        if (response.Error is not null)
        {
            dispatcher.Dispatch(new MemberUpdateFailedAction(response.Error));

            return;
        }

        dispatcher.Dispatch(new MemberUpdateSucceededAction(response.Content!));
        dispatcher.Dispatch(new MembersRequestedAction(action.HomeId));
    }

    [EffectMethod]
    public async Task HandleMemberDeleted(MemberDeletedAction action, IDispatcher dispatcher)
    {
        var response = await _members.DeleteMember(action.HomeId, action.MemberId);

        if (response.Error is not null)
        {
            dispatcher.Dispatch(new MemberDeletionFailedAction(response.Error));

            return;
        }

        dispatcher.Dispatch(new MemberDeletionSucceededAction(response.Content!));
        dispatcher.Dispatch(new MembersRequestedAction(action.HomeId));
    }

    [EffectMethod]
    public async Task HandleMemberActivated(MemberActivatedAction action, IDispatcher dispatcher)
    {
        var response = await _members.ActivateMember(action.HomeId, action.MemberId);

        if (response.Error is not null)
        {
            dispatcher.Dispatch(new MemberUpdateFailedAction(response.Error));

            return;
        }

        dispatcher.Dispatch(new MemberUpdateSucceededAction(response.Content!));
        dispatcher.Dispatch(new MembersRequestedAction(action.HomeId));
    }
}

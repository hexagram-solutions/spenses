using Fluxor;
using Spenses.Client.Http;
using Spenses.Web.Client.Infrastructure;
using Spenses.Web.Client.Store.Shared;

namespace Spenses.Web.Client.Store.Members;

public class Effects(IMembersApi members)
{
    [EffectMethod]
    public async Task HandleMembersRequested(MembersRequestedAction action, IDispatcher dispatcher)
    {
        var response = await members.GetMembers(action.HomeId);

        if (response.Error is not null)
        {
            dispatcher.Dispatch(new MembersRequestFailedAction());
            dispatcher.Dispatch(new ApplicationErrorAction(response.Error.ToApplicationError()));

            return;
        }

        dispatcher.Dispatch(new MembersReceivedAction(response.Content!));
    }

    [EffectMethod]
    public async Task HandleMemberRequested(MemberRequestedAction action, IDispatcher dispatcher)
    {
        var response = await members.GetMember(action.HomeId, action.MemberId);

        if (response.Error is not null)
        {
            dispatcher.Dispatch(new MemberRequestFailedAction());
            dispatcher.Dispatch(new ApplicationErrorAction(response.Error.ToApplicationError()));

            return;
        }

        dispatcher.Dispatch(new MemberReceivedAction(response.Content!));
    }

    [EffectMethod]
    public async Task HandleMemberCreated(MemberCreatedAction action, IDispatcher dispatcher)
    {
        var response = await members.PostMember(action.HomeId, action.Props);

        if (response.Error is not null)
        {
            dispatcher.Dispatch(new MemberCreationFailedAction());
            dispatcher.Dispatch(new ApplicationErrorAction(response.Error.ToApplicationError()));

            return;
        }

        dispatcher.Dispatch(new MemberCreationSucceededAction(response.Content!));
        dispatcher.Dispatch(new MembersRequestedAction(action.HomeId));
    }

    [EffectMethod]
    public async Task HandleMemberUpdated(MemberUpdatedAction action, IDispatcher dispatcher)
    {
        var response = await members.PutMember(action.HomeId, action.MemberId, action.Props);

        if (response.Error is not null)
        {
            dispatcher.Dispatch(new MemberUpdateFailedAction());
            dispatcher.Dispatch(new ApplicationErrorAction(response.Error.ToApplicationError()));

            return;
        }

        dispatcher.Dispatch(new MemberUpdateSucceededAction(response.Content!));
        dispatcher.Dispatch(new MembersRequestedAction(action.HomeId));
    }

    [EffectMethod]
    public async Task HandleMemberDeleted(MemberDeletedAction action, IDispatcher dispatcher)
    {
        var response = await members.DeleteMember(action.HomeId, action.MemberId);

        if (response.Error is not null)
        {
            dispatcher.Dispatch(new MemberDeletionFailedAction());
            dispatcher.Dispatch(new ApplicationErrorAction(response.Error.ToApplicationError()));

            return;
        }

        dispatcher.Dispatch(new MemberDeletionSucceededAction(response.Content!));
        dispatcher.Dispatch(new MembersRequestedAction(action.HomeId));
    }

    [EffectMethod]
    public async Task HandleMemberActivated(MemberActivatedAction action, IDispatcher dispatcher)
    {
        var response = await members.ActivateMember(action.HomeId, action.MemberId);

        if (response.Error is not null)
        {
            dispatcher.Dispatch(new MemberUpdateFailedAction());
            dispatcher.Dispatch(new ApplicationErrorAction(response.Error.ToApplicationError()));

            return;
        }

        dispatcher.Dispatch(new MemberUpdateSucceededAction(response.Content!));
        dispatcher.Dispatch(new MembersRequestedAction(action.HomeId));
    }
}

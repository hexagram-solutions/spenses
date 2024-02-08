using Fluxor;
using MudBlazor;
using Refit;
using Spenses.App.Infrastructure;
using Spenses.App.Store.Shared;
using Spenses.Client.Http;

namespace Spenses.App.Store.Members;

public class Effects(IMembersApi members, ISnackbar snackbar)
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

        if (response.Error is not null &&
            await response.Error.GetContentAsAsync<ProblemDetails>() is { } problemDetails)
        {
            dispatcher.Dispatch(new MemberCreationFailedAction(problemDetails.Errors ?? []));

            return;
        }

        if (!response.IsSuccessStatusCode)
        {
            dispatcher.Dispatch(new MemberCreationFailedAction([]));
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

        if (response.Error is not null &&
            await response.Error.GetContentAsAsync<ProblemDetails>() is { } problemDetails)
        {
            dispatcher.Dispatch(new MemberUpdateFailedAction(problemDetails.Errors ?? []));

            return;
        }

        if (!response.IsSuccessStatusCode)
        {
            dispatcher.Dispatch(new MemberUpdateFailedAction([]));
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
            dispatcher.Dispatch(new MemberActivationFailedAction());
            dispatcher.Dispatch(new ApplicationErrorAction(response.Error.ToApplicationError()));

            return;
        }

        dispatcher.Dispatch(new MemberActivationSucceededAction(response.Content!));
        dispatcher.Dispatch(new MembersRequestedAction(action.HomeId));
    }

    [EffectMethod]
    public async Task HandleMemberInvited(MemberInvitedAction action, IDispatcher dispatcher)
    {
        var response = await members.PostMemberInvitation(action.HomeId, action.MemberId, action.Props);

        if (response.Error is not null)
        {
            dispatcher.Dispatch(new MemberInvitationFailedAction());
            dispatcher.Dispatch(new ApplicationErrorAction(response.Error.ToApplicationError()));

            return;
        }

        dispatcher.Dispatch(new MemberInvitationSucceededAction(response.Content!));
    }

    [EffectMethod]
    public Task HandleMemberInvitationSucceeded(MemberInvitationSucceededAction action, IDispatcher _)
    {
        snackbar.Add($"Invitation sent to {action.Invitation.Email}", Severity.Success);

        return Task.CompletedTask;
    }

    [EffectMethod]
    public async Task HandleMemberInvitationsCancelled(MemberInvitationsCancelledAction action, IDispatcher dispatcher)
    {
        var response = await members.CancelMemberInvitations(action.HomeId, action.MemberId);

        if (response.Error is not null)
        {
            dispatcher.Dispatch(new MemberInvitationsCancellationFailedAction());
            dispatcher.Dispatch(new ApplicationErrorAction(response.Error.ToApplicationError()));

            return;
        }

        dispatcher.Dispatch(new MemberInvitationsCancellationSucceededAction(action.MemberId));
    }

    [EffectMethod]
    public Task HandleMemberInvitationsCancellationSucceeded(MemberInvitationsCancellationSucceededAction _,
        IDispatcher __)
    {
        snackbar.Add("Invitation cancelled.", Severity.Info);

        return Task.CompletedTask;
    }
}

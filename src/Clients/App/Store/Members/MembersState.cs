using FluentValidation.Results;
using Fluxor;
using Spenses.Shared.Models.Members;

namespace Spenses.App.Store.Members;

[FeatureState(Name = "Members", CreateInitialStateMethodName = nameof(Initialize))]
public record MembersState
{
    private static MembersState Initialize()
    {
        return new MembersState();
    }

    public Member? CurrentMember { get; init; }

    public Member[] Members { get; init; } = [];

    public Dictionary<string, string[]> Errors { get; set; } = [];

    public bool MembersRequesting { get; init; }

    public bool MemberRequesting { get; init; }

    public bool MemberCreating { get; init; }

    public bool MemberUpdating { get; init; }

    public bool MemberDeleting { get; init; }

    public bool MemberActivating { get; init; }

    public bool MemberInviting { get; init; }

    public bool MemberInvitationsCancelling { get; init; }
}

using Fluxor;
using Refit;
using Spenses.Application.Models.Members;

namespace Spenses.Client.Web.Store.Members;

[FeatureState(Name = "Members", CreateInitialStateMethodName = nameof(Initialize))]
public record MembersState
{
    private static MembersState Initialize()
    {
        return new MembersState();
    }

    public Member? CurrentMember { get; init; }

    public Member[] Members { get; init; } = Array.Empty<Member>();

    public bool MembersRequesting { get; init; }

    public bool MemberRequesting { get; init; }

    public bool MemberCreating { get; init; }

    public bool MemberUpdating { get; init; }

    public bool MemberDeleting { get; init; }

    public bool MemberActivating { get; init; }

    public ApiException? Error { get; init; }
}

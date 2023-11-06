using BlazorState;
using Spenses.Application.Models.Members;

namespace Spenses.Client.Web.Features.Members;

public partial class MembersState : State<MembersState>
{
    public Member? CurrentMember { get; private set; }

    public bool MemberCreating { get; private set; }

    public bool MemberUpdating { get; private set; }

    public bool MemberRequesting { get; private set; }

    public bool MemberDeleting { get; private set; }

    public IEnumerable<Member>? Members { get; private set; }

    public bool MembersRequesting { get; private set; }

    public override void Initialize()
    {
        Members = Enumerable.Empty<Member>();
    }
}

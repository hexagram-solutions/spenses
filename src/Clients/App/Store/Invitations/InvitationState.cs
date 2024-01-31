using Fluxor;
using Spenses.Shared.Models.Invitations;

namespace Spenses.App.Store.Invitations;

[FeatureState(Name = "Identity", CreateInitialStateMethodName = nameof(Initialize))]
public record InvitationState
{
    public Invitation? Invitation { get; init; }

    public bool InvitationRequesting { get; init; }

    public bool InvitationResponding { get; init; }

    private static InvitationState Initialize()
    {
        return new InvitationState();
    }
}

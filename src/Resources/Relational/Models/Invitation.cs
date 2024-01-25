namespace Spenses.Resources.Relational.Models;

public class Invitation : AggregateRoot
{
    public string Email { get; set; } = null!;

    public InvitationStatus Status { get; set; }

    public Guid? MemberId { get; set; }

    public Member? Member { get; set; }

    public Guid HomeId { get; set; }

    public Home Home { get; set; } = null!;
}

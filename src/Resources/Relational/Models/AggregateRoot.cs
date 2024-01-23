namespace Spenses.Resources.Relational.Models;

public abstract class AggregateRoot : Entity
{
    public DateTime CreatedAt { get; set; }

    public DateTime ModifiedAt { get; set; }

    public Guid CreatedById { get; set; }

    public ApplicationUser CreatedBy { get; set; } = null!;

    public Guid ModifiedById { get; set; }

    public ApplicationUser ModifiedBy { get; set; } = null!;
}

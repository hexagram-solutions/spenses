namespace Spenses.Resources.Relational.Models;

public abstract class AggregateRoot : Entity
{
    public DateTime CreatedAt { get; set; }

    public DateTime ModifiedAt { get; set; }

    public string CreatedById { get; set; } = null!;

    public ApplicationUser CreatedBy { get; set; } = null!;

    public string ModifiedById { get; set; } = null!;

    public ApplicationUser ModifiedBy { get; set; } = null!;
}

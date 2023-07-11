using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Spenses.Resources.Relational.Models;

public abstract class Entity
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public Guid Id { get; set; } = Guid.NewGuid();
}

public class Home : Entity
{
    public string Name { get; set; } = null!;

    public string? Description { get; set; }
}

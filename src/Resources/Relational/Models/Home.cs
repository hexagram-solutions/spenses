namespace Spenses.Resources.Relational.Models;

public class Home : Entity
{
    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public ICollection<Member> Members { get; set; } = new List<Member>();
}

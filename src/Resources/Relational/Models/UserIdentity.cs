using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Spenses.Resources.Relational.Models;

public class UserIdentity
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public string Id { get; set; } = null!;

    public string NickName { get; set; } = null!;

    public string Issuer { get; set; } = null!;

    [EmailAddress]
    public string Email { get; set; } = null!;

    public ICollection<Credit> CreatedCredits { get; set; } = new HashSet<Credit>();

    public ICollection<Credit> ModifiedCredits { get; set; } = new HashSet<Credit>();

    public ICollection<Expense> CreatedExpenses { get; set; } = new HashSet<Expense>();

    public ICollection<Expense> ModifiedExpenses { get; set; } = new HashSet<Expense>();

    public ICollection<Home> CreatedHomes { get; set; } = new HashSet<Home>();

    public ICollection<Home> ModifiedHomes { get; set; } = new HashSet<Home>();

    public ICollection<Member> CreatedMembers { get; set; } = new HashSet<Member>();

    public ICollection<Member> ModifiedMembers { get; set; } = new HashSet<Member>();
}

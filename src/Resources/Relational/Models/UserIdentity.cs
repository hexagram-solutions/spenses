using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Spenses.Resources.Relational.Models;

public class UserIdentity
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public string Id { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string Issuer { get; set; } = null!;

    [EmailAddress]
    public string Email { get; set; } = null!;
}

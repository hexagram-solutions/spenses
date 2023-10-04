using System.ComponentModel.DataAnnotations;
using Spenses.Application.Models.Users;

namespace Spenses.Application.Models;

public interface IAggregateRoot
{
    [Required]
    public Guid Id { get; set; }

    [Required]
    public User CreatedBy { get; set; }

    [Required]
    public DateTime CreatedAt { get; set; }

    [Required]
    public User ModifiedBy { get; set; }

    [Required]
    public DateTime ModifiedAt { get; set; }
}

using System.ComponentModel.DataAnnotations;

namespace Spenses.Shared.Models.Authentication;

public record RegisterRequest(
    [EmailAddress] string Email,
    [Required]string Password);

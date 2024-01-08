using System.ComponentModel.DataAnnotations;

namespace Spenses.Shared.Models.Authentication;

public record ResendConfirmationEmailRequest(
    [Required] [EmailAddress] string Email);

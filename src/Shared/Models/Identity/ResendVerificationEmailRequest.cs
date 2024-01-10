using System.ComponentModel.DataAnnotations;

namespace Spenses.Shared.Models.Identity;

public record ResendVerificationEmailRequest(
    [Required][EmailAddress] string Email);

using System.ComponentModel.DataAnnotations;

namespace Spenses.Shared.Models.Identity;

public record VerifyEmailRequest(
    [Required] string UserId,
    [Required] string Code,
    string? NewEmail = null);

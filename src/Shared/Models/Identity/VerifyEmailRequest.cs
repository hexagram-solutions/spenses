using System.ComponentModel.DataAnnotations;

namespace Spenses.Shared.Models.Identity;

public record VerifyEmailRequest(
    [Required] Guid UserId,
    [Required] string Code,
    string? NewEmail = null);

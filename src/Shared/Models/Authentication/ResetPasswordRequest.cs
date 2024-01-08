using System.ComponentModel.DataAnnotations;

namespace Spenses.Shared.Models.Authentication;

public record ResetPasswordRequest(
    [EmailAddress] [Required] string Email,
    [Required] string ResetCode,
    [Required] string NewPassword);

using System.ComponentModel.DataAnnotations;

namespace Spenses.Shared.Models.Authentication;

public record LoginResult(
    [Required] bool Succeeded,
    [Required] bool RequiresTwoFactor = false,
    [Required] bool IsNotAllowed = false,
    [Required] bool IsLockedOut = false);

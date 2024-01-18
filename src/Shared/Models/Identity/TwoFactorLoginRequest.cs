using System.ComponentModel.DataAnnotations;

namespace Spenses.Shared.Models.Identity;

public record TwoFactorLoginRequest(string? Code, string? RecoveryCode, [Required] bool RememberClient);

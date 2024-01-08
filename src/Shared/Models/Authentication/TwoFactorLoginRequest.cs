using System.ComponentModel.DataAnnotations;

namespace Spenses.Shared.Models.Authentication;

public record TwoFactorLoginRequest(string? Code, string? RecoveryCode, [Required] bool RememberClient);

using System.ComponentModel.DataAnnotations;

namespace Spenses.Shared.Models.Identity;

public record ForgotPasswordRequest(
    [Required][EmailAddress] string Email);

using System.ComponentModel.DataAnnotations;

namespace Spenses.Shared.Models.Authentication;

public record ForgotPasswordRequest(
    [Required][EmailAddress] string Email);

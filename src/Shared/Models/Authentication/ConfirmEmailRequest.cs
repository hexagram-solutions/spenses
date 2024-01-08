using System.ComponentModel.DataAnnotations;

namespace Spenses.Shared.Models.Authentication;

public record ConfirmEmailRequest(
    [Required] string UserId,
    [Required] string Code);

using System.ComponentModel.DataAnnotations;

namespace Spenses.Shared.Models.Authentication;

public record LoginRequest(
    [EmailAddress] string Email,
    string Password);
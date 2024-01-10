using Spenses.Shared.Models.Identity;

namespace Spenses.App.Store.Identity;

public record LoginRequestedAction(LoginRequest Request, string? ReturnUrl = null);

public record LoginSucceededAction;

public record LoginFailedAction(string Error);

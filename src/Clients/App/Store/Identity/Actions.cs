using Spenses.Shared.Models.Identity;

namespace Spenses.App.Store.Identity;

public record LoginRequestedAction(LoginRequest Request, string? ReturnUrl = null);

public record LoginSucceededAction;

public record LoginFailedAction(string Error);

public record RegistrationRequestedAction(RegisterRequest Request);

public record RegistrationSucceededAction;

public record RegistrationFailedAction(string[] Errors);

public record EmailVerificationRequestedAction(VerifyEmailRequest Request);

public record EmailVerificationSucceededAction;

public record EmailVerificationFailedAction(string Error);

public record ResendVerificationEmailRequestedAction(ResendVerificationEmailRequest Request);

public record ResendVerificationEmailSucceededAction;

public record ResendVerificationEmailFailedAction(string Error);

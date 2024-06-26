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

public record EmailVerificationFailedAction;

public record ResendVerificationEmailRequestedAction(string Email);

public record ResendVerificationEmailSucceededAction;

public record ResendVerificationEmailFailedAction;

public record LogoutRequestedAction;

public record LogoutSucceededAction;

public record LogoutFailedAction;

public record ForgotPasswordRequestedAction(string Email);

public record ForgotPasswordSucceededAction;

public record ForgotPasswordFailedAction;

public record ResetPasswordRequestedAction(ResetPasswordRequest Request);

public record ResetPasswordSucceededAction;

public record ResetPasswordFailedAction(string[] Errors);

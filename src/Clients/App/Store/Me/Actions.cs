using Spenses.Shared.Models.Me;

namespace Spenses.App.Store.Me;

public record CurrentUserRequestedAction;

public record CurrentUserReceivedAction(CurrentUser CurrentUser);

public record CurrentUserRequestFailedAction;

public record CurrentUserUpdatedAction(UserProfileProperties Props);

public record CurrentUserUpdateSucceededAction(CurrentUser CurrentUser);

public record CurrentUserUpdateFailedAction;

public record ChangeEmailRequestedAction(ChangeEmailRequest Request);

public record ChangeEmailSucceededAction;

public record ChangeEmailFailedAction(string[] Errors);

public record ChangePasswordRequestedAction(ChangePasswordRequest Request);

public record ChangePasswordSucceededAction;

public record ChangePasswordFailedAction(string[] Errors);

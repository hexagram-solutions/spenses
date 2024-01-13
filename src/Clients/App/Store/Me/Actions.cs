using Spenses.Shared.Models.Me;

namespace Spenses.App.Store.Me;

public record CurrentUserRequestedAction;

public record CurrentUserReceivedAction(CurrentUser CurrentUser);

public record CurrentUserRequestFailedAction;

public record CurrentUserUpdatedAction(UserProfileProperties Props);

public record CurrentUserUpdateSucceededAction(CurrentUser CurrentUser);

public record CurrentUserUpdateFailedAction;

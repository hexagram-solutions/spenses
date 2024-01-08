using Spenses.Shared.Models.Homes;

namespace Spenses.Web.Client.Store.Homes;

public record HomesRequestedAction;

public record HomesReceivedAction(Home[] Homes);

public record HomesRequestFailedAction;

public record HomeRequestedAction(Guid HomeId);

public record HomeReceivedAction(Home Home);

public record HomeRequestFailedAction;

public record HomeCreatedAction(HomeProperties Props);

public record HomeCreationSucceededAction(Home Home);

public record HomeCreationFailedAction;

public record HomeUpdatedAction(Guid HomeId, HomeProperties Props);

public record HomeUpdateSucceededAction(Home Home);

public record HomeUpdateFailedAction;

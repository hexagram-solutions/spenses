using Spenses.Application.Models.Homes;

namespace Spenses.Client.Web.Store.Homes;

public record HomesRequestedAction;

public record HomesReceivedAction(IEnumerable<Home> Homes);

public record HomesRequestFailedAction(string Message);

public record HomeRequestedAction(Guid HomeId);

public record HomeReceivedAction(Home Home);

public record HomeRequestFailedAction(string Message);

public record HomeCreatedAction(HomeProperties Props);

public record HomeCreationSucceededAction(Home Home);

public record HomeCreationFailedAction(string Message);

public record HomeUpdatedAction(Guid HomeId, HomeProperties Props);

public record HomeUpdateSucceededAction(Home Home);

public record HomeUpdateFailedAction(string Message);

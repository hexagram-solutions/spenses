using Refit;
using Spenses.Application.Models.Homes;

namespace Spenses.Client.Web.Store.Homes;

public record HomesRequestedAction;

public record HomesReceivedAction(IEnumerable<Home> Homes);

public record HomesRequestFailedAction(ApiException Error);

public record HomeRequestedAction(Guid HomeId);

public record HomeReceivedAction(Home Home);

public record HomeRequestFailedAction(ApiException Error);

public record HomeCreatedAction(HomeProperties Props);

public record HomeCreationSucceededAction(Home Home);

public record HomeCreationFailedAction(ApiException Error);

public record HomeUpdatedAction(Guid HomeId, HomeProperties Props);

public record HomeUpdateSucceededAction(Home Home);

public record HomeUpdateFailedAction(ApiException Error);

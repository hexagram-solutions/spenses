using Spenses.Application.Models.Homes;

namespace Spenses.Client.Web.Store.Homes;

public record HomesRequestedAction;

public record HomesReceivedAction(IEnumerable<Home> Homes);

public record HomesRequestFailedAction(string Message);

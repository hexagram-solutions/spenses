using Spenses.Shared.Models.Insights;

namespace Spenses.Client.Web.Store.Insights;

public record ExpensesOverTimeRequestedAction(Guid HomeId, ExpenseDateGrouping Grouping);

public record ExpensesOverTimeReceivedAction(ExpenseTotalItem[] Items);

public record ExpensesOverTimeRequestFailedAction;



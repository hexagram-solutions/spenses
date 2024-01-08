using Spenses.Shared.Models.Insights;

namespace Spenses.Web.Client.Store.Insights;

public record ExpensesOverTimeRequestedAction(Guid HomeId, ExpenseDateGrouping Grouping);

public record ExpensesOverTimeReceivedAction(ExpenseTotalItem[] Items);

public record ExpensesOverTimeRequestFailedAction;



using Spenses.Shared.Models.Homes;

namespace Spenses.App.Store.Dashboard;

// TODO: Don't love needing the home id here, seems bad
public record DashboardPeriodChangedAction(Guid HomeId, DateOnly Start, DateOnly End);

public record BalanceBreakdownRequestedAction(Guid HomeId);

public record BalanceBreakdownReceivedAction(BalanceBreakdown BalanceBreakdown);

public record BalanceBreakdownRequestFailedAction;

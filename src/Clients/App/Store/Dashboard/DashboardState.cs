using Fluxor;
using Spenses.Shared.Models.Homes;

namespace Spenses.App.Store.Dashboard;

[FeatureState(Name = "Dashboard", CreateInitialStateMethodName = nameof(Initialize))]
public record DashboardState
{
    private static DashboardState Initialize()
    {
        var today = DateTime.Today;

        var daysInMonth = DateTime.DaysInMonth(today.Year, today.Month);

        return new DashboardState
        {
            PeriodStart = new DateOnly(today.Year, today.Month, 1),
            PeriodEnd = new DateOnly(today.Year, today.Month, daysInMonth)
        };
    }

    public DateOnly PeriodStart { get; init; }

    public DateOnly PeriodEnd { get; init; }

    public BalanceBreakdown? BalanceBreakdown { get; init; }

    public bool BalanceBreakdownRequesting { get; init; }
}

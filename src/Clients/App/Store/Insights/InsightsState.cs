using Fluxor;
using Spenses.Shared.Models.Insights;

namespace Spenses.App.Store.Insights;

[FeatureState(Name = "Insights", CreateInitialStateMethodName = nameof(Initialize))]
public record InsightsState
{
    private static InsightsState Initialize()
    {
        return new InsightsState();
    }

    public ExpenseTotalItem[] ExpensesOverTime { get; init; } = [];

    public bool ExpensesOverTimeRequesting { get; init; }
}

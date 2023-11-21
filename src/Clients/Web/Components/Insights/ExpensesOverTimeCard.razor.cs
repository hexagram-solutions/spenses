using Blazorise.Charts;
using Fluxor;
using Microsoft.AspNetCore.Components;
using Spenses.Application.Models.Insights;
using Spenses.Client.Web.Store.Insights;

namespace Spenses.Client.Web.Components.Insights;

public partial class ExpensesOverTimeCard
{
    [Parameter]
    public Guid HomeId { get; set; }

    [Inject]
    private IState<InsightsState> InsightsState { get; set; } = null!;

    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;

    private BarChart<decimal> ChartRef { get; set; } = null!;

    private static readonly BarChartOptions Options = new()
    {
        Responsive = true,
        Plugins = new ChartPlugins
        {
            Tooltips = new ChartTooltips
            {
                CornerRadius = 10,
            }
        }
    };

    protected override void OnInitialized()
    {
        base.OnInitialized();

        Dispatcher.Dispatch(new ExpensesOverTimeRequestedAction(HomeId, ExpenseDateGrouping.Month));

        SubscribeToAction<ExpensesOverTimeReceivedAction>(async _ =>
        {
            await HandleRedraw();
        });
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await HandleRedraw();
        }
    }

    private async Task HandleRedraw()
    {
        await ChartRef.Clear();

        if (InsightsState.Value.ExpensesOverTimeRequesting)
            return;

        await ChartRef.AddLabelsDatasetsAndUpdate(
            InsightsState.Value.ExpensesOverTime.Select(i => i.Date.ToString("MMMM yyyy")).ToList(),
            new BarChartDataset<decimal>
            {
                Label = "Total expenses",
                BackgroundColor = Color.Info,
                BorderColor = Color.Primary,
                BorderWidth = 1,
                Data = InsightsState.Value.ExpensesOverTime.Select(i => i.Total).ToList()
            });
    }
}

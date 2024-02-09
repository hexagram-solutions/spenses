using Fluxor;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor;
using Spenses.App.Store.Payments;
using Spenses.Client.Http;
using Spenses.Shared.Models.Payments;
using SortDirection = Spenses.Shared.Models.Common.SortDirection;

namespace Spenses.App.Components.Payments;

public partial class PaymentsGrid
{
    [Parameter]
    public Guid HomeId { get; init; }

    [Inject]
    private IState<PaymentsState> PaymentsState { get; init; } = null!;

    [Inject]
    private IDispatcher Dispatcher { get; init; } = null!;

    [Inject]
    private IDialogService DialogService { get; init; } = null!;

    [Inject]
    private IPaymentsApi PaymentsApi { get; init; } = null!;

    private MudDataGrid<PaymentDigest> DataGridRef { get; set; } = new();
    private IDialogReference? CreateDialog { get; set; }
    private IDialogReference? EditDialog { get; set; }

    protected override void OnInitialized()
    {
        base.OnInitialized();

        SubscribeToAction<PaymentCreationSucceededAction>(async _ =>
        {
            CreateDialog?.Close();
            await DataGridRef.ReloadServerData();
        });

        SubscribeToAction<PaymentUpdateSucceededAction>(async _ =>
        {
            EditDialog?.Close();
            await DataGridRef.ReloadServerData();
        });

        SubscribeToAction<PaymentDeletionSucceededAction>(async _ => { await DataGridRef.ReloadServerData(); });
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);

        if (firstRender)
            await DataGridRef.SetSortAsync(nameof(PaymentDigest.Date), MudBlazor.SortDirection.Descending, x => x.Date);
    }

    private async Task CreatePayment()
    {
        CreateDialog = await DialogService.ShowAsync<CreatePaymentDialog>();
    }

    private async Task OnEditClicked(MouseEventArgs _, Guid paymentId)
    {
        var parameters =
            new DialogParameters<EditPaymentDialog> { { x => x.PaymentId, paymentId } };

        EditDialog = await DialogService.ShowAsync<EditPaymentDialog>("Edit member", parameters);
    }

    private async Task OnDeleteClicked(MouseEventArgs _, PaymentDigest payment)
    {
        var confirmed = await DialogService.ShowMessageBox(
            "Are you sure you want to delete this payment?",
            $"{payment.Amount} paid by {payment.PaidByMemberName} on {payment.Date:O}",
            "Delete payment",
            cancelText: "Close");

        if (confirmed != true)
            return;

        Dispatcher.Dispatch(new PaymentDeletedAction(HomeId, payment.Id));
    }

    private FilteredPaymentsQuery Query { get; set; } = new()
    {
        OrderBy = nameof(PaymentDigest.Date),
        SortDirection = SortDirection.Desc
    };

    private DateRange? DateRangeValue
    {
        get
        {
            if (!Query.MinDate.HasValue && !Query.MaxDate.HasValue)
                return null;

            var today = DateOnly.FromDateTime(DateTime.Today);

            return new DateRange(
                new DateTime(Query.MinDate.GetValueOrDefault(today), TimeOnly.MinValue),
                new DateTime(Query.MaxDate.GetValueOrDefault(today), TimeOnly.MinValue));
        }
    }

    private Task OnDateFilterChanged(DateRange? range)
    {
        if (range is null)
        {
            Query.MinDate = null;
            Query.MaxDate = null;

            return Task.CompletedTask;
        }

        Query.MinDate = range.Start.HasValue ? DateOnly.FromDateTime(range.Start.GetValueOrDefault()) : null;
        Query.MaxDate = range.End.HasValue ? DateOnly.FromDateTime(range.End.GetValueOrDefault()) : null;

        return DataGridRef.ReloadServerData();
    }

    private async Task<GridData<PaymentDigest>> GetServerData(GridState<PaymentDigest> state)
    {
        var sortDefinition = state.SortDefinitions.SingleOrDefault();

        if (sortDefinition is not null)
        {
            Query.SortDirection = sortDefinition.Descending ? SortDirection.Desc : SortDirection.Asc;
            Query.OrderBy = sortDefinition.SortBy;
        }

        var response = await PaymentsApi.GetPayments(HomeId, Query);

        return new GridData<PaymentDigest>
        {
            Items = response.Content!.Items,
            TotalItems = response.Content.TotalCount
        };
    }
}

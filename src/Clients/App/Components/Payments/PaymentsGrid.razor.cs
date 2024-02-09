using Fluxor;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor;
using Spenses.App.Components.Shared;
using Spenses.App.Store.Payments;
using Spenses.Client.Http;
using Spenses.Shared.Models.Payments;
using SortDirection = Spenses.Shared.Models.Common.SortDirection;

namespace Spenses.App.Components.Payments;

public partial class PaymentsGrid
{
    public PaymentsGrid()
    {
        var today = DateTime.Today;

        var daysInMonth = DateTime.DaysInMonth(today.Year, today.Month);

        Query = new FilteredPaymentsQuery
        {
            OrderBy = nameof(PaymentDigest.Date),
            SortDirection = SortDirection.Desc,
            MinDate = new DateOnly(today.Year, today.Month, 1),
            MaxDate = new DateOnly(today.Year, today.Month, daysInMonth)
        };
    }

    [Parameter]
    public Guid HomeId { get; init; }

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

        this.SubscribeToAsyncAction<PaymentCreationSucceededAction>(_ =>
        {
            CreateDialog?.Close();
            return DataGridRef.ReloadServerData();
        });

        this.SubscribeToAsyncAction<PaymentUpdateSucceededAction>(_ =>
        {
            EditDialog?.Close();
            return DataGridRef.ReloadServerData();
        });

        this.SubscribeToAsyncAction<PaymentDeletionSucceededAction>(_ => DataGridRef.ReloadServerData());
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

    private FilteredPaymentsQuery Query { get; }

    private DateRange Period => new(
        new DateTime(Query.MinDate, TimeOnly.MinValue),
        new DateTime(Query.MaxDate, TimeOnly.MinValue));

    private Task OnPeriodFilterChanged(DateRange range)
    {
        Query.MinDate = DateOnly.FromDateTime(range.Start.GetValueOrDefault());
        Query.MaxDate = DateOnly.FromDateTime(range.End.GetValueOrDefault());

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

using Blazorise.DataGrid;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Spenses.Application.Models.Common;
using Spenses.Application.Models.Payments;
using Spenses.Client.Http;
using Spenses.Client.Web.Features.Payments;
using SortDirection = Spenses.Application.Models.Common.SortDirection;

namespace Spenses.Client.Web.Components.Payments;

public partial class PaymentsGrid
{
    [Parameter]
    public Guid HomeId { get; set; }

    [Inject]
    private IPaymentsApi PaymentsApi { get; init; } = null!;

    [Inject]
    public IModalService ModalService { get; set; } = null!;

    [Inject]
    public IMessageService MessageService { get; set; } = null!;

    private DataGrid<PaymentDigest> DataGridRef { get; set; } = new();

    private IReadOnlyList<DateOnly?>? FilterDates { get; set; }

    private VirtualizeOptions VirtualizeOptions { get; set; } = new() { DataGridHeight = "750px" };

    private PagedResult<PaymentDigest> Payments { get; set; } = new(0, Enumerable.Empty<PaymentDigest>());

    private FilteredPaymentQuery Query { get; set; } = new()
    {
        OrderBy = nameof(PaymentDigest.Date),
        SortDirection = SortDirection.Desc
    };

    private Task OnDataGridSortChanged(DataGridSortChangedEventArgs args)
    {
        Query.SortDirection = args.SortDirection switch
        {
            Blazorise.SortDirection.Default => null,
            Blazorise.SortDirection.Ascending => SortDirection.Asc,
            Blazorise.SortDirection.Descending => SortDirection.Desc,
            _ => throw new ArgumentOutOfRangeException(nameof(args), args.SortDirection, null)
        };

        Query.OrderBy = args.FieldName;

        return DataGridRef.Reload();
    }

    private async Task OnDataGridReadData(DataGridReadDataEventArgs<PaymentDigest> args)
    {
        Query.Skip = args.VirtualizeOffset;
        Query.Take = args.VirtualizeCount;

        Payments = (await PaymentsApi.GetPayments(HomeId, Query)).Content!;
    }

    private Task OnDatesFilterChanged(IReadOnlyList<DateOnly?> dateValues)
    {
        var dates = dateValues.OrderBy(x => x).ToList();

        Query.MinDate = dates.FirstOrDefault();
        Query.MaxDate = dates.LastOrDefault();

        FilterDates = dateValues;

        return DataGridRef.Reload();
    }

    private Task ClearFilters()
    {
        Query.MinDate = null;
        Query.MaxDate = null;

        FilterDates = null;

        return DataGridRef.Reload();
    }
    private Task OnPaymentSaved()
    {
        return DataGridRef.Reload();
    }

    private Task OnAddPaymentClicked()
    {
        return ModalService.Show<CreatePaymentModal>(p => p.Add(x => x.OnSave, OnPaymentSaved));
    }


    private Task OnEditPaymentClicked(MouseEventArgs _, Guid paymentId)
    {
        return ModalService.Show<EditPaymentModal>(p =>
        {
            p.Add(x => x.PaymentId, paymentId);
            p.Add(x => x.OnSave, OnPaymentSaved);
        });
    }

    private async Task OnDeletePaymentClicked(MouseEventArgs _, PaymentDigest payment)
    {
        var confirmed = await MessageService.Confirm(
            $"{payment.Amount} paid by {payment.PaidByMemberName} on {payment.Date:O}",
            "Are you sure you want to delete this payment?");

        if (!confirmed)
            return;

        await Mediator.Send(new PaymentsState.PaymentDeleted(HomeId, payment.Id));

        await DataGridRef.Reload();
    }
}

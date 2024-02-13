using Refit;
using Spenses.Client.Http;
using Spenses.Shared.Models.Common;
using Spenses.Shared.Models.Payments;

namespace Spenses.Api.IntegrationTests.Payments;

public partial class PaymentsIntegrationTests : IdentityIntegrationTestBase
{
    private readonly IHomesApi _homes;
    private readonly IPaymentsApi _payments;

    public PaymentsIntegrationTests(DatabaseFixture databaseFixture, AuthenticationFixture authFixture)
        : base(databaseFixture, authFixture)
    {
        _homes = CreateApiClient<IHomesApi>();
        _payments = CreateApiClient<IPaymentsApi>();
    }

    private FilteredPaymentsQuery DefaultPaymentsQuery
    {
        get
        {
            var today = DateTime.Today;

            var daysInMonth = DateTime.DaysInMonth(today.Year, today.Month);

            return new FilteredPaymentsQuery
            {
                OrderBy = nameof(PaymentDigest.Date),
                SortDirection = SortDirection.Desc,
                MinDate = new DateOnly(today.Year, today.Month, 1),
                MaxDate = new DateOnly(today.Year, today.Month, daysInMonth)
            };
        }
    }
}

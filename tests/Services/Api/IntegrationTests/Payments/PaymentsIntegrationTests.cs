using Refit;
using Spenses.Client.Http;
using Spenses.Shared.Models.Common;
using Spenses.Shared.Models.Payments;

namespace Spenses.Api.IntegrationTests.Payments;

[Collection(IdentityWebApplicationCollection.CollectionName)]
public partial class PaymentsIntegrationTests(IdentityWebApplicationFixture<Program> fixture) : IAsyncLifetime
{
    private readonly IHomesApi _homes = RestService.For<IHomesApi>(fixture.CreateAuthenticatedClient());

    private readonly IPaymentsApi _payments =
        RestService.For<IPaymentsApi>(fixture.CreateAuthenticatedClient());

    public Task InitializeAsync()
    {
        return Task.CompletedTask;
    }

    public async Task DisposeAsync()
    {
        await fixture.LoginAsTestUser();
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

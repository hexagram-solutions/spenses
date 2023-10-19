using System.Security.Claims;
using FakeItEasy;
using Hexagrams.Extensions.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Spenses.Application.Features.Homes;
using Spenses.Application.Features.Homes.Requests;
using Spenses.Utilities.Security;
using Spenses.Utilities.Security.Services;
using DbModels = Spenses.Resources.Relational.Models;

namespace Spenses.Application.Tests.Features.Homes.Requests;

public class BalanceBreakdownQueryHandlerTests : IAsyncDisposable
{
    private const string TestUserId = "test";

    private readonly IDbContextFactory _dbFactory;

    public BalanceBreakdownQueryHandlerTests()
    {
        var fakeCurrentUserService = A.Fake<ICurrentUserService>();

        A.CallTo(() => fakeCurrentUserService.CurrentUser).Returns(new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
            new Claim(ApplicationClaimTypes.Identifier, TestUserId)
        })));

        _dbFactory = new InMemoryDbContextFactory(fakeCurrentUserService);
    }

    public async ValueTask DisposeAsync()
    {
        await _dbFactory.DisposeAsync();
    }

    [Fact]
    public async Task Balance_breakdown_has_correct_balances()
    {
        await ArrangeData();

        async Task TestAction(BalanceBreakdownQueryHandler handler)
        {
            await using var db = _dbFactory.Create();

            var home = await db.Homes
                .Include(h => h.Members)
                .Include(h => h.Expenses)
                .Include(h => h.Payments)
                .SingleAsync();

            var today = DateOnly.FromDateTime(DateTime.Today);
            var query = new BalanceBreakdownQuery(home.Id, today, today);

            var balanceBreakdown = await handler.Handle(query, CancellationToken.None);

            var allExpensesSum = home.Expenses.Sum(e => e.Amount);
            var allPaymentsSum = home.Payments.Sum(c => c.Amount);

            balanceBreakdown.TotalExpenses.Should().Be(allExpensesSum);
            balanceBreakdown.TotalPayments.Should().Be(allPaymentsSum);
            balanceBreakdown.TotalBalance.Should().Be(allExpensesSum - allPaymentsSum);

            foreach (var memberBalance in balanceBreakdown.MemberBalances)
            {
                var dbMember = home.Members.Single(m => m.Id == memberBalance.OwedByMember.Id);
                var memberPaid = dbMember.Payments.Sum(c => c.Amount);

                memberBalance.TotalPaid.Should().Be(memberPaid);

                var expectedMemberOwed = allExpensesSum * memberBalance.OwedByMember.DefaultSplitPercentage;

                memberBalance.TotalOwed.Should().BeApproximately(expectedMemberOwed, 0.01m);
            }
        }

        await ServiceTestHarness<BalanceBreakdownQueryHandler>.Create(TestAction)
            .WithServices(services =>
            {
                services.AddAutoMapper(typeof(HomesMappingProfile));
                services.AddScoped(_ => _dbFactory.Create());
            })
            .TestAsync();
    }

    private async Task ArrangeData()
    {
        await using var db = _dbFactory.Create();

        var today = DateOnly.FromDateTime(DateTime.Today);

        await db.Users.AddAsync(new DbModels.UserIdentity
        {
            Id = TestUserId,
            Email = "test@example.com",
            NickName = "test",
            Issuer = "self"
        });

        var homeEntry = await db.Homes.AddAsync(new DbModels.Home { Name = "Test home" });

        await db.Members.AddRangeAsync(
            new DbModels.Member
            {
                Name = "Hingle McCringleberry",
                DefaultSplitPercentage = 0.33m,
                HomeId = homeEntry.Entity.Id
            },
            new DbModels.Member
            {
                Name = "Grunky Peep",
                DefaultSplitPercentage = 0.66m,
                HomeId = homeEntry.Entity.Id
            });

        await db.SaveChangesAsync();

        foreach (var member in db.Members)
        {
            for (var i = 0; i < 3; i++)
            {
                await db.Expenses.AddAsync(new DbModels.Expense
                {
                    Date = today,
                    Amount = 100.00m,
                    PaidByMemberId = member.Id,
                    HomeId = homeEntry.Entity.Id
                });

                await db.Payments.AddAsync(new DbModels.Payment
                {
                    Date = today,
                    Amount = 50.00m,
                    PaidByMemberId = member.Id,
                    HomeId = homeEntry.Entity.Id
                });
            }
        }

        await db.SaveChangesAsync();
    }
}

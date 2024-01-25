using System.Security.Claims;
using FakeItEasy;
using Hexagrams.Extensions.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Spenses.Application.Features.Homes;
using Spenses.Application.Features.Homes.Requests;
using Spenses.Shared.Models.Homes;
using Spenses.Utilities.Security;
using Spenses.Utilities.Security.Services;
using DbModels = Spenses.Resources.Relational.Models;

namespace Spenses.Application.Tests.Features.Homes.Requests;

public class BalanceBreakdownQueryHandlerTests : IAsyncDisposable
{
    private readonly Guid _testUserId = Guid.Parse("00000000-0000-0000-0000-000000000001");

    private readonly IDbContextFactory _dbFactory;

    public BalanceBreakdownQueryHandlerTests()
    {
        var fakeCurrentUserService = A.Fake<ICurrentUserService>();

        A.CallTo(() => fakeCurrentUserService.CurrentUser).Returns(new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
            new Claim(ApplicationClaimTypes.Identifier, _testUserId.ToString())
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
                    .ThenInclude(member => member.PaymentsPaid)
                .Include(h => h.Members)
                    .ThenInclude(member => member.PaymentsReceived)
                .Include(h => h.Expenses)
                    .ThenInclude(e => e.ExpenseShares)
                .Include(h => h.Payments)
                .SingleAsync();

            var today = DateOnly.FromDateTime(DateTime.Today);
            var query = new BalanceBreakdownQuery(home.Id, today, today);

            var balanceBreakdown = await handler.Handle(query, CancellationToken.None);

            var allExpensesSum = home.Expenses.Sum(e => e.Amount);

            balanceBreakdown.TotalExpenses.Should().Be(allExpensesSum);

            foreach (var memberBalance in balanceBreakdown.MemberBalances)
            {
                memberBalance.Debts.Single().Should().BeEquivalentTo(new MemberDebt
                {
                    OwedTo = balanceBreakdown.MemberBalances.Select(mb => mb.Member)
                        .Single(m => m.Id != memberBalance.Member.Id),
                    BalanceOwing = 75.00m,
                    TotalOwed = 150.00m,
                    TotalPaid = 75.00m
                });
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

        await db.Users.AddAsync(new DbModels.ApplicationUser
        {
            Id = _testUserId,
            Email = "test@example.com",
            UserName = "test@example.com",
            DisplayName = "test",
            AvatarUrl = "https://image.com"
        });

        var homeEntry = await db.Homes.AddAsync(new DbModels.Home
        {
            Name = "Test home",
            CreatedById = _testUserId,
            ModifiedById = _testUserId
        });

        var oneThirdMember = (await db.Members.AddAsync(
            new DbModels.Member
            {
                Name = "Hingle McCringleberry",
                DefaultSplitPercentage = 0.50m,
                HomeId = homeEntry.Entity.Id,
                CreatedById = _testUserId,
                ModifiedById = _testUserId
            })).Entity;

        var twoThirdsMember = (await db.Members.AddAsync(
            new DbModels.Member
            {
                Name = "Grunky Peep",
                DefaultSplitPercentage = 0.50m,
                HomeId = homeEntry.Entity.Id,
                CreatedById = _testUserId,
                ModifiedById = _testUserId
            })).Entity;

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
                    HomeId = homeEntry.Entity.Id,
                    ExpenseShares =
                    {
                        new DbModels.ExpenseShare
                        {
                            OwedByMemberId = oneThirdMember.Id,
                            OwedAmount = 50.00m,
                            OwedPercentage = 50.00m,
                        },
                        new DbModels.ExpenseShare
                        {
                            OwedByMemberId = twoThirdsMember.Id,
                            OwedAmount = 50.00m,
                            OwedPercentage = 50.00m
                        }
                    },
                    Category = new DbModels.ExpenseCategory
                    {
                        Name = Guid.NewGuid().ToString(),
                        HomeId = homeEntry.Entity.Id,
                        CreatedById = _testUserId,
                        ModifiedById = _testUserId
                    },
                    CreatedById = _testUserId,
                    ModifiedById = _testUserId
                });

                await db.Payments.AddAsync(new DbModels.Payment
                {
                    Date = today,
                    Amount = 25.00m,
                    PaidByMemberId = member.Id,
                    PaidToMemberId = db.Members.First(m => m.Id != member.Id).Id,
                    HomeId = homeEntry.Entity.Id,
                    CreatedById = _testUserId,
                    ModifiedById = _testUserId
                });
            }
        }

        await db.SaveChangesAsync();
    }
}

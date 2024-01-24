using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Spenses.Application.Behaviors;
using Spenses.Application.Features.Homes.Authorization;
using Spenses.Resources.Relational;
using Spenses.Shared.Models.Homes;
using Spenses.Shared.Models.Members;

namespace Spenses.Application.Features.Homes.Requests;

public record BalanceBreakdownQuery(Guid HomeId, DateOnly PeriodStart, DateOnly PeriodEnd)
    : IAuthorizedRequest<BalanceBreakdown>
{
    public AuthorizationPolicy Policy => Policies.MemberOfHomePolicy(HomeId);
}

public class BalanceBreakdownQueryHandler(ApplicationDbContext db, IMapper mapper)
    : IRequestHandler<BalanceBreakdownQuery, BalanceBreakdown>
{
    public async Task<BalanceBreakdown> Handle(BalanceBreakdownQuery request, CancellationToken cancellationToken)
    {
        var (homeId, periodStart, periodEnd) = request;

        var home = await db.Homes
            .Include(h => h.Expenses.Where(e => e.Date >= periodStart && e.Date <= periodEnd))
                .ThenInclude(e => e.ExpenseShares)
            .Include(h => h.Members)
                .ThenInclude(member => member.PaymentsPaid.Where(e => e.Date >= periodStart && e.Date <= periodEnd))
            .Include(h => h.Members)
                .ThenInclude(member => member.User)
            .FirstAsync(h => h.Id == homeId, cancellationToken);

        var membersById = home.Members.ToDictionary(k => k.Id, v => v);

        var memberBalances = membersById.Values.Select(member =>
        {
            // Get all the expense shares for expenses that the current member did *not* pay for
            var otherMemberExpenseShares = home.Expenses
                .Where(e => e.PaidByMemberId != member.Id)
                .SelectMany(e => e.ExpenseShares)
                // Filter out expense shares for the current member because they can't owe themselves
                .Where(e => e.OwedByMemberId != member.Id)
                .GroupBy(x => x.OwedByMemberId)
                .ToList();

            // Aggregate the expense shares owed to other home members
            var debts = new List<MemberDebt>();

            foreach (var expenseShareGroup in otherMemberExpenseShares)
            {
                var totalOwedToOtherMember = expenseShareGroup.Sum(expenseShare => expenseShare.OwedAmount);

                var totalPaidToOtherMember = member.PaymentsPaid
                    .Where(p => p.PaidToMemberId == expenseShareGroup.Key)
                    .Sum(p => p.Amount);

                debts.Add(new MemberDebt
                {
                    OwedTo = mapper.Map<Member>(membersById[expenseShareGroup.Key]),
                    TotalOwed = totalOwedToOtherMember,
                    TotalPaid = totalPaidToOtherMember,
                    BalanceOwing = totalOwedToOtherMember - totalPaidToOtherMember
                });
            }

            var totalOwingByMember = debts.Sum(d => d.BalanceOwing);
            var totalPaidByMember = member.PaymentsPaid.Sum(e => e.Amount);

            return new MemberBalance
            {
                Member = mapper.Map<Member>(member),
                TotalOwing = totalOwingByMember,
                TotalPaid = totalPaidByMember,
                Balance = totalOwingByMember - totalPaidByMember,
                Debts = [.. debts]
            };
        }).ToArray();

        return new BalanceBreakdown
        {
            TotalExpenses = home.Expenses.Sum(e => e.Amount),
            MemberBalances = memberBalances
        };
    }
}

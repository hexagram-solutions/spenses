using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Spenses.Application.Exceptions;
using Spenses.Application.Models.Homes;
using Spenses.Application.Models.Members;
using Spenses.Resources.Relational;

namespace Spenses.Application.Features.Homes.Requests;

public record BalanceBreakdownQuery(Guid HomeId, DateOnly PeriodStart, DateOnly PeriodEnd) : IRequest<BalanceBreakdown>;

public class BalanceBreakdownQueryHandler(ApplicationDbContext db, IMapper mapper)
    : IRequestHandler<BalanceBreakdownQuery, BalanceBreakdown>
{
    public async Task<BalanceBreakdown> Handle(BalanceBreakdownQuery request, CancellationToken cancellationToken)
    {
        var (homeId, periodStart, periodEnd) = request;

        var home = await db.Homes
            .Include(h => h.Expenses.Where(e => e.Date >= periodStart && e.Date <= periodEnd))
                .ThenInclude(e => e.ExpenseShares)
            .Include(h => h.Payments.Where(e => e.Date >= periodStart && e.Date <= periodEnd))
            .Include(h => h.Members)
            .FirstOrDefaultAsync(h => h.Id == homeId, cancellationToken);

        if (home is null)
            throw new ResourceNotFoundException(homeId);

        var totalExpenses = home.Expenses.Sum(e => e.Amount);
        var totalPayments = home.Payments.Sum(e => e.Amount);

        var totalBalance = totalExpenses - totalPayments;

        var members = home.Members;

        return new BalanceBreakdown
        {
            TotalExpenses = totalExpenses,
            TotalPayments = totalPayments,
            TotalBalance = totalBalance,
            MemberBalances = members.Select(m =>
            {
                var owedByMember = home.Expenses
                    .SelectMany(e => e.ExpenseShares)
                    .Where(es => es.OwedByMemberId == m.Id)
                    .Sum(es => es.OwedAmount);

                var paidByMember = m.PaymentsPaid.Sum(e => e.Amount);

                return new MemberBalance
                {
                    Member = mapper.Map<Member>(m),
                    TotalOwed = owedByMember,
                    TotalPaid = paidByMember,
                    Balance = owedByMember - paidByMember
                };
            })
        };
    }
}

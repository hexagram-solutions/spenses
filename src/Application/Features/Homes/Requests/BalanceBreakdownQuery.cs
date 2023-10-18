using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Spenses.Application.Exceptions;
using Spenses.Application.Models.Homes;
using Spenses.Application.Models.Members;
using Spenses.Resources.Relational;

namespace Spenses.Application.Features.Homes.Requests;

public record BalanceBreakdownQuery(Guid HomeId, DateOnly PeriodStart, DateOnly PeriodEnd) : IRequest<BalanceBreakdown>;

public class BalanceBreakdownQueryHandler : IRequestHandler<BalanceBreakdownQuery, BalanceBreakdown>
{
    private readonly ApplicationDbContext _db;
    private readonly IMapper _mapper;

    public BalanceBreakdownQueryHandler(ApplicationDbContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    public async Task<BalanceBreakdown> Handle(BalanceBreakdownQuery request, CancellationToken cancellationToken)
    {
        var (homeId, periodStart, periodEnd) = request;

        var members = await _db.Members
            .Where(m => m.HomeId == homeId)
            .Include(m => m.Expenses.Where(e => e.Date >= periodStart && e.Date <= periodEnd))
            .Include(m => m.Payments.Where(e => e.Date >= periodStart && e.Date <= periodEnd))
            .ToListAsync(cancellationToken);

        if (Math.Abs(members.Sum(m => m.DefaultSplitPercentage) - 1) > 0.1)
            throw new InvalidRequestException("Split percentage among home members is less than 100%.");

        var totalExpenses = members.SelectMany(m => m.Expenses).Sum(e => e.Amount);
        var totalPayments = members.SelectMany(m => m.Payments).Sum(e => e.Amount);

        var totalBalance = totalExpenses - totalPayments;

        return new BalanceBreakdown
        {
            TotalExpenses = totalExpenses,
            TotalPayments = totalPayments,
            TotalBalance = totalBalance,
            MemberBalances = members.Select(m =>
            {
                var owedByMember = Math.Round(totalExpenses * new decimal(m.DefaultSplitPercentage), 2);
                var paidByMember = m.Payments.Sum(e => e.Amount);

                return new MemberBalance
                {
                    OwedByMember = _mapper.Map<Member>(m),
                    TotalOwed = owedByMember,
                    TotalPaid = paidByMember,
                    Balance = owedByMember - paidByMember
                };
            })
        };
    }
}

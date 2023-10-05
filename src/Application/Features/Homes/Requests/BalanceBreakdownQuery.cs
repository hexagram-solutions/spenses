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
            .Include(m => m.Credits.Where(e => e.Date >= periodStart && e.Date <= periodEnd))
            .ToListAsync(cancellationToken);

        // All homes will have at least one member, so if no members are found we know the home doesn't exist.
        if (!members.Any())
            throw new ResourceNotFoundException(request.HomeId);

        if (Math.Abs(members.Sum(m => m.SplitPercentage) - 1) > 0.1)
            throw new InvalidRequestException("Split percentage among home members is less than 100%.");

        var totalExpenses = members.SelectMany(m => m.Expenses).Sum(e => e.Amount);
        var totalCredits = members.SelectMany(m => m.Credits).Sum(e => e.Amount);

        var totalBalance = totalExpenses - totalCredits;

        return new BalanceBreakdown
        {
            TotalExpenses = totalExpenses,
            TotalCredits = totalCredits,
            TotalBalance = totalBalance,
            MemberBalances = members.Select(m =>
            {
                var owedByMember = Math.Round(totalExpenses * new decimal(m.SplitPercentage), 2);
                var paidByMember = m.Credits.Sum(e => e.Amount);

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

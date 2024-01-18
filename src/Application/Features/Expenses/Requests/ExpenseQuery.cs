using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Spenses.Application.Behaviors;
using Spenses.Application.Exceptions;
using Spenses.Application.Features.Homes.Authorization;
using Spenses.Resources.Relational;
using Spenses.Shared.Models.Expenses;

namespace Spenses.Application.Features.Expenses.Requests;

public record ExpenseQuery(Guid HomeId, Guid ExpenseId) : IAuthorizedRequest<Expense>
{
    public AuthorizationPolicy Policy => Policies.MemberOfHomePolicy(HomeId);
}

public class ExpenseQueryCommandHandler(ApplicationDbContext db, IMapper mapper)
    : IRequestHandler<ExpenseQuery, Expense>
{
    public async Task<Expense> Handle(ExpenseQuery request, CancellationToken cancellationToken)
    {
        var (homeId, expenseId) = request;

        var expense = await db.Expenses
            .Where(e => e.Home.Id == homeId)
            .ProjectTo<Expense>(mapper.ConfigurationProvider)
            .FirstOrDefaultAsync(h => h.Id == expenseId, cancellationToken);

        return expense ?? throw new ResourceNotFoundException(expenseId);
    }
}


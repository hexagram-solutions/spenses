using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Spenses.Application.Common.Behaviors;
using Spenses.Application.Exceptions;
using Spenses.Application.Features.Homes.Authorization;
using Spenses.Application.Models.Expenses;
using Spenses.Resources.Relational;

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
        var expense = await db.Expenses
            .ProjectTo<Expense>(mapper.ConfigurationProvider)
            .FirstOrDefaultAsync(h => h.Id == request.ExpenseId, cancellationToken);

        return expense ?? throw new ResourceNotFoundException(request.ExpenseId);
    }
}


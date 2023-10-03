using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Spenses.Application.Common.Behaviors;
using Spenses.Application.Exceptions;
using Spenses.Application.Features.Homes.Authorization;
using Spenses.Application.Models;
using Spenses.Resources.Relational;

namespace Spenses.Application.Features.Expenses.Requests;

public record ExpenseQuery(Guid HomeId, Guid ExpenseId) : IAuthorizedRequest<Expense>
{
    public AuthorizationPolicy Policy => Policies.MemberOfHomePolicy(HomeId);
}

public class ExpenseQueryCommandHandler : IRequestHandler<ExpenseQuery, Expense>
{
    private readonly ApplicationDbContext _db;
    private readonly IMapper _mapper;

    public ExpenseQueryCommandHandler(ApplicationDbContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    public async Task<Expense> Handle(ExpenseQuery request, CancellationToken cancellationToken)
    {
        var (homeId, expenseId) = request;

        var expense = await _db.Expenses
            .Where(e => e.Home.Id == homeId)
            .ProjectTo<Expense>(_mapper.ConfigurationProvider)
            .FirstOrDefaultAsync(h => h.Id == expenseId, cancellationToken);

        return expense ?? throw new ResourceNotFoundException(expenseId);
    }
}


using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Spenses.Application.Authorization;
using Spenses.Application.Common.Results;
using Spenses.Application.Models;
using Spenses.Resources.Relational;

namespace Spenses.Application.Features.Homes.Expenses;

public record ExpenseQuery(Guid HomeId, Guid ExpenseId) : IAuthorizedRequest<ServiceResult<Expense>>
{
    public AuthorizationPolicy Policy => Policies.MemberOfHomePolicy(HomeId);

    public ServiceResult<Expense> OnUnauthorized() => new UnauthorizedErrorResult();
}

public class ExpenseQueryCommandHandler : IRequestHandler<ExpenseQuery, ServiceResult<Expense>>
{
    private readonly ApplicationDbContext _db;
    private readonly IMapper _mapper;

    public ExpenseQueryCommandHandler(ApplicationDbContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    public async Task<ServiceResult<Expense>> Handle(ExpenseQuery request, CancellationToken cancellationToken)
    {
        var (homeId, expenseId) = request;

        var expense = await _db.Expenses
            .Where(e => e.Home.Id == homeId)
            .ProjectTo<Expense>(_mapper.ConfigurationProvider)
            .FirstOrDefaultAsync(h => h.Id == expenseId, cancellationToken);

        return expense is null ? new NotFoundErrorResult(expenseId) : expense;
    }
}


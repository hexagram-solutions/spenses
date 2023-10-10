using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Spenses.Application.Common.Behaviors;
using Spenses.Application.Exceptions;
using Spenses.Application.Features.Homes.Authorization;
using Spenses.Application.Models.ExpenseCategories;
using Spenses.Resources.Relational;

namespace Spenses.Application.Features.ExpenseCategories.Requests;

public record ExpenseCategoryQuery(Guid HomeId, Guid ExpenseCategoryId) : IAuthorizedRequest<ExpenseCategory>
{
    public AuthorizationPolicy Policy => Policies.MemberOfHomePolicy(HomeId);
}

public class ExpenseCategoryQueryCommandHandler : IRequestHandler<ExpenseCategoryQuery, ExpenseCategory>
{
    private readonly ApplicationDbContext _db;
    private readonly IMapper _mapper;

    public ExpenseCategoryQueryCommandHandler(ApplicationDbContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    public async Task<ExpenseCategory> Handle(ExpenseCategoryQuery request, CancellationToken cancellationToken)
    {
        var (homeId, expenseId) = request;

        var expense = await _db.ExpenseCategories
            .Where(e => e.Home.Id == homeId)
            .ProjectTo<ExpenseCategory>(_mapper.ConfigurationProvider)
            .FirstOrDefaultAsync(h => h.Id == expenseId, cancellationToken);

        return expense ?? throw new ResourceNotFoundException(expenseId);
    }
}


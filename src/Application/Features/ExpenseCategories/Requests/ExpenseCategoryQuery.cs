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

public class ExpenseCategoryQueryCommandHandler(ApplicationDbContext db, IMapper mapper)
    : IRequestHandler<ExpenseCategoryQuery, ExpenseCategory>
{
    public async Task<ExpenseCategory> Handle(ExpenseCategoryQuery request, CancellationToken cancellationToken)
    {
        var (homeId, categoryId) = request;

        var category = await db.ExpenseCategories
            .Where(e => e.Home.Id == homeId)
            .ProjectTo<ExpenseCategory>(mapper.ConfigurationProvider)
            .FirstOrDefaultAsync(h => h.Id == categoryId, cancellationToken);

        return category ?? throw new ResourceNotFoundException(categoryId);
    }
}


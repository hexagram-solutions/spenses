using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Spenses.Application.Behaviors;
using Spenses.Application.Features.Homes.Authorization;
using Spenses.Resources.Relational;
using Spenses.Shared.Models.ExpenseCategories;

namespace Spenses.Application.Features.ExpenseCategories.Requests;

public record ExpenseCategoriesQuery(Guid HomeId) : IAuthorizedRequest<IEnumerable<ExpenseCategory>>
{
    public AuthorizationPolicy Policy => Policies.MemberOfHomePolicy(HomeId);
}

public class ExpenseCategoriesQueryHandler(ApplicationDbContext db, IMapper mapper)
    : IRequestHandler<ExpenseCategoriesQuery, IEnumerable<ExpenseCategory>>
{
    public async Task<IEnumerable<ExpenseCategory>> Handle(ExpenseCategoriesQuery request,
        CancellationToken cancellationToken)
    {
        var categories = await db.ExpenseCategories
            .Where(ec => ec.HomeId == request.HomeId)
            .OrderByDescending(ec => ec.IsDefault)
            .ThenBy(ec => ec.Name)
            .ProjectTo<ExpenseCategory>(mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken);

        return categories;
    }
}

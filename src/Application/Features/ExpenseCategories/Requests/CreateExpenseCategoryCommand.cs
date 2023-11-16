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
using DbModels = Spenses.Resources.Relational.Models;

namespace Spenses.Application.Features.ExpenseCategories.Requests;

public record CreateExpenseCategoryCommand(Guid HomeId, ExpenseCategoryProperties Props)
    : IAuthorizedRequest<ExpenseCategory>
{
    public AuthorizationPolicy Policy => Policies.MemberOfHomePolicy(HomeId);
}

public class CreateExpenseCategoryCommandHandler(ApplicationDbContext db, IMapper mapper)
    : IRequestHandler<CreateExpenseCategoryCommand, ExpenseCategory>
{
    public async Task<ExpenseCategory> Handle(CreateExpenseCategoryCommand request, CancellationToken cancellationToken)
    {
        var (homeId, props) = request;

        var home = await db.Homes
            .Include(h => h.ExpenseCategories)
            .FirstAsync(h => h.Id == homeId, cancellationToken);

        if (home.ExpenseCategories.Any(m => m.Name == props.Name))
            throw new InvalidRequestException($"A category named {props.Name} already exists.");

        var category = mapper.Map<DbModels.ExpenseCategory>(props);

        home.ExpenseCategories.Add(category);

        await db.SaveChangesAsync(cancellationToken);

        var createdCategory = await db.ExpenseCategories
            .ProjectTo<ExpenseCategory>(mapper.ConfigurationProvider)
            .FirstAsync(e => e.Id == category.Id, cancellationToken);

        return createdCategory;
    }
}

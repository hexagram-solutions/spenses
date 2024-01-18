using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Spenses.Application.Behaviors;
using Spenses.Application.Exceptions;
using Spenses.Application.Features.Homes.Authorization;
using Spenses.Resources.Relational;
using Spenses.Shared.Models.ExpenseCategories;

namespace Spenses.Application.Features.ExpenseCategories.Requests;

public record UpdateExpenseCategoryCommand(Guid HomeId, Guid ExpenseCategoryId, ExpenseCategoryProperties Props)
    : IAuthorizedRequest<ExpenseCategory>
{
    public AuthorizationPolicy Policy => Policies.MemberOfHomePolicy(HomeId);
}

public class UpdateExpenseCategoryCommandHandler(ApplicationDbContext db, IMapper mapper)
    : IRequestHandler<UpdateExpenseCategoryCommand, ExpenseCategory>
{
    public async Task<ExpenseCategory> Handle(UpdateExpenseCategoryCommand request, CancellationToken cancellationToken)
    {
        var (homeId, categoryId, props) = request;

        var home = await db.Homes
            .Include(h => h.ExpenseCategories)
            .FirstAsync(h => h.Id == homeId, cancellationToken);

        if (home.ExpenseCategories.FirstOrDefault(ec => ec.Id == categoryId) is not { } category)
            throw new ResourceNotFoundException(categoryId);

        if (category.IsDefault)
            throw new InvalidRequestException("A home's default expense category cannot be modified.");

        if (home.ExpenseCategories.Any(m => m.Name == props.Name && m.Id != categoryId))
            throw new InvalidRequestException($"A category named {props.Name} already exists.");

        mapper.Map(props, category);

        await db.SaveChangesAsync(cancellationToken);

        var updatedCategory = await db.ExpenseCategories
            .ProjectTo<ExpenseCategory>(mapper.ConfigurationProvider)
            .FirstAsync(ec => ec.Id == categoryId, cancellationToken);

        return updatedCategory;
    }
}

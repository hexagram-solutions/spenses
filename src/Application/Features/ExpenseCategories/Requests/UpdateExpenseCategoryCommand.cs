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

        var category = await db.ExpenseCategories
            .Where(e => e.Home.Id == homeId)
            .FirstOrDefaultAsync(e => e.Id == categoryId, cancellationToken);

        if (category is null)
            throw new ResourceNotFoundException(categoryId);

        if (category.IsDefault)
            throw new InvalidRequestException("A home's default expense category cannot be modified.");

        mapper.Map(props, category);

        await db.SaveChangesAsync(cancellationToken);

        var updatedCategory = await db.ExpenseCategories
            .ProjectTo<ExpenseCategory>(mapper.ConfigurationProvider)
            .FirstAsync(ec => ec.Id == categoryId, cancellationToken);

        return updatedCategory;
    }
}

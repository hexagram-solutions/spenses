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

public record UpdateExpenseCategoryCommand(Guid HomeId, Guid ExpenseCategoryId, ExpenseCategoryProperties Props)
    : IAuthorizedRequest<ExpenseCategory>
{
    public AuthorizationPolicy Policy => Policies.MemberOfHomePolicy(HomeId);
}

public class UpdateExpenseCategoryCommandHandler : IRequestHandler<UpdateExpenseCategoryCommand, ExpenseCategory>
{
    private readonly ApplicationDbContext _db;
    private readonly IMapper _mapper;

    public UpdateExpenseCategoryCommandHandler(ApplicationDbContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    public async Task<ExpenseCategory> Handle(UpdateExpenseCategoryCommand request, CancellationToken cancellationToken)
    {
        var (homeId, categoryId, props) = request;

        var category = await _db.ExpenseCategories
            .Where(e => e.Home.Id == homeId)
            .FirstOrDefaultAsync(e => e.Id == categoryId, cancellationToken);

        if (category is null)
            throw new ResourceNotFoundException(categoryId);

        _mapper.Map(props, category);

        await _db.SaveChangesAsync(cancellationToken);

        var updatedCategory = await _db.ExpenseCategories
            .ProjectTo<ExpenseCategory>(_mapper.ConfigurationProvider)
            .FirstAsync(ec => ec.Id == categoryId, cancellationToken);

        return updatedCategory;
    }
}

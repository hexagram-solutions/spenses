using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Spenses.Application.Common.Behaviors;
using Spenses.Application.Features.Homes.Authorization;
using Spenses.Application.Models.ExpenseCategories;
using Spenses.Resources.Relational;

namespace Spenses.Application.Features.ExpenseCategories.Requests;

public record ExpenseCategoriesQuery(Guid HomeId) : IAuthorizedRequest<IEnumerable<ExpenseCategory>>
{
    public AuthorizationPolicy Policy => Policies.MemberOfHomePolicy(HomeId);
}

public class ExpenseCategoriesQueryHandler : IRequestHandler<ExpenseCategoriesQuery, IEnumerable<ExpenseCategory>>
{
    private readonly ApplicationDbContext _db;
    private readonly IMapper _mapper;

    public ExpenseCategoriesQueryHandler(ApplicationDbContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    public async Task<IEnumerable<ExpenseCategory>> Handle(ExpenseCategoriesQuery request,
        CancellationToken cancellationToken)
    {
        var categories = await _db.ExpenseCategories
            .OrderBy(x => x.Name)
            .ProjectTo<ExpenseCategory>(_mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken);

        return categories;
    }
}

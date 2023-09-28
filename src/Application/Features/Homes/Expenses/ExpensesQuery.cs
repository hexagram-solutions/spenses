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

public record ExpensesQuery(Guid HomeId) : IAuthorizedRequest<ServiceResult<IEnumerable<Expense>>>
{
    public AuthorizationPolicy Policy => Policies.MemberOfHomePolicy(HomeId);

    public ServiceResult<IEnumerable<Expense>> OnUnauthorized() => new UnauthorizedErrorResult();
}

public class ExpensesQueryHandler : IRequestHandler<ExpensesQuery, ServiceResult<IEnumerable<Expense>>>
{
    private readonly ApplicationDbContext _db;
    private readonly IMapper _mapper;

    public ExpensesQueryHandler(ApplicationDbContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    public async Task<ServiceResult<IEnumerable<Expense>>> Handle(ExpensesQuery request,
        CancellationToken cancellationToken)
    {
        var expenses = await _db.Expenses
            .Where(e => e.Home.Id == request.HomeId)
            .OrderByDescending(h => h.Date)
            .ProjectTo<Expense>(_mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken);

        return expenses;
    }
}

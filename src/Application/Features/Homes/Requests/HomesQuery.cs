using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Spenses.Resources.Relational;
using Spenses.Shared.Models.Homes;
using Spenses.Utilities.Security;
using Spenses.Utilities.Security.Services;
using DbModels = Spenses.Resources.Relational.Models;

namespace Spenses.Application.Features.Homes.Requests;

public record HomesQuery : IRequest<IEnumerable<Home>>;

public class HomesQueryHandler(ApplicationDbContext db, IMapper mapper, IUserContext userContext)
    : IRequestHandler<HomesQuery, IEnumerable<Home>>
{
    public async Task<IEnumerable<Home>> Handle(HomesQuery request, CancellationToken cancellationToken)
    {
        var currentUserId = userContext.CurrentUser.GetId();

        var homes = await db.Homes
            .Where(h => h.Members.Any(m => m.Status == DbModels.MemberStatus.Active && m.UserId == currentUserId))
            .ProjectTo<Home>(mapper.ConfigurationProvider)
            .OrderBy(h => h.Name)
            .ToListAsync(cancellationToken);

        return homes;
    }
}

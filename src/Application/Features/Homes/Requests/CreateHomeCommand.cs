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

public record CreateHomeCommand(HomeProperties Props) : IRequest<Home>;

public class CreateHomeCommandHandler(ApplicationDbContext db, IMapper mapper, ICurrentUserService currentUserService)
    : IRequestHandler<CreateHomeCommand, Home>
{
    public async Task<Home> Handle(CreateHomeCommand request, CancellationToken cancellationToken)
    {
        var home = mapper.Map<DbModels.Home>(request.Props);

        var currentUser = currentUserService.CurrentUser!;

        home.Members.Add(new DbModels.Member
        {
            Name = currentUser.FindFirst(ApplicationClaimTypes.NickName)!.Value,
            ContactEmail = currentUser.FindFirst(ApplicationClaimTypes.Email)!.Value,
            DefaultSplitPercentage = 1m,
            UserId = currentUser.GetId()
        });

        home.ExpenseCategories.Add(new DbModels.ExpenseCategory
        {
            Name = "General (uncategorized)",
            Description = "Catch-all category for expenses that don't fit another category. This category cannot be " +
                "changed or removed.",
            IsDefault = true
        });

        var entry = await db.Homes.AddAsync(home, cancellationToken);

        await db.SaveChangesAsync(cancellationToken);

        var createdHome = await db.Homes
            .ProjectTo<Home>(mapper.ConfigurationProvider)
            .SingleAsync(h => h.Id == entry.Entity.Id, cancellationToken);

        return mapper.Map<Home>(createdHome);
    }
}

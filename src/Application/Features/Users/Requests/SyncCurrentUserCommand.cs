using AutoMapper;
using MediatR;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Spenses.Application.Models.Users;
using Spenses.Resources.Relational;
using Spenses.Resources.Relational.Models;
using Spenses.Utilities.Security;
using Spenses.Utilities.Security.Services;

namespace Spenses.Application.Features.Users.Requests;

public record SyncCurrentUserCommand : IRequest<User>;

public class SyncCurrentUserCommandHandler(ApplicationDbContext db, ICurrentUserService currentUserService,
        IMapper mapper)
    : IRequestHandler<SyncCurrentUserCommand, User>
{
    public async Task<User> Handle(SyncCurrentUserCommand request, CancellationToken cancellationToken)
    {
        var currentUser = currentUserService.CurrentUser;
        var currentUserId = currentUser.GetId();

        var userIdentity = await db.Users.FirstOrDefaultAsync(c => c.Id == currentUserId, cancellationToken);

        if (userIdentity is null)
        {
            userIdentity = mapper.Map<ApplicationUser>(currentUser);

            var currentUserEntry = await db.Users.AddAsync(userIdentity, cancellationToken);

            userIdentity = currentUserEntry.Entity;
        }
        else
        {
            mapper.Map(currentUser, userIdentity);
        }

        try
        {
            await db.SaveChangesAsync(cancellationToken);
        }
        // Handle the scenario where the current user was added moments ago on a parallel request. In this case, there
        // is no need to update the user since they were just created, so do nothing.
        catch (DbUpdateException ex) when (ex.InnerException is SqlException { Number: 2601 })
        {
            // Error number 2601 represents a duplicate key violation: "Cannot insert duplicate key row in object '%.*ls'
            // with unique index '%.*ls'."
            // https://docs.microsoft.com/en-us/sql/relational-databases/errors-events/database-engine-events-and-errors#errors-2000-to-2999
        }

        return mapper.Map<User>(userIdentity);
    }
}

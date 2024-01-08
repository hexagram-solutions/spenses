using AutoMapper;
using FluentValidation.Results;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Spenses.Application.Common.Behaviors;
using Spenses.Application.Exceptions;
using Spenses.Application.Features.Homes.Authorization;
using Spenses.Resources.Relational;
using Spenses.Shared.Models.Members;
using DbModels = Spenses.Resources.Relational.Models;

namespace Spenses.Application.Features.Members.Requests;

public record CreateMemberCommand(Guid HomeId, MemberProperties Props) : IAuthorizedRequest<Member>
{
    public AuthorizationPolicy Policy => Policies.MemberOfHomePolicy(HomeId);
}

public class CreateMemberCommandHandler(ApplicationDbContext db, IMapper mapper)
    : IRequestHandler<CreateMemberCommand, Member>
{
    public async Task<Member> Handle(CreateMemberCommand request, CancellationToken cancellationToken)
    {
        var (homeId, props) = request;

        var home = await db.Homes
            .Include(h => h.Members)
            .FirstAsync(h => h.Id == homeId, cancellationToken);

        var otherMembersSplitPercentageTotal = home.Members.Sum(m => m.DefaultSplitPercentage);

        if (otherMembersSplitPercentageTotal + props.DefaultSplitPercentage > 1)
        {
            throw new InvalidRequestException(
                new ValidationFailure(nameof(MemberProperties.DefaultSplitPercentage),
                    "Total split percentage among home members cannot exceed 100%"));
        }

        var member = mapper.Map<DbModels.Member>(props);

        home.Members.Add(member);

        await db.SaveChangesAsync(cancellationToken);

        return mapper.Map<Member>(db.Entry(member).Entity);
    }
}

using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Spenses.Application.Models;
using Spenses.Resources.Relational;

namespace Spenses.Application.Features.Homes.Members;

public class CreateMemberCommandValidator : AbstractValidator<CreateMemberCommand>
{
    private readonly ApplicationDbContext _db;

    public CreateMemberCommandValidator(ApplicationDbContext db)
    {
        _db = db;

        RuleFor(x => x.Props)
            .SetValidator(new MemberPropertiesValidator());

        RuleFor(x => x)
            .MustAsync(SplitPercentageCannotExceedMaximumAmongHomeMembers);
    }

    // TODO: Would it be better to just do this in the command handler?
    public async Task<bool> SplitPercentageCannotExceedMaximumAmongHomeMembers(CreateMemberCommand command,
        CancellationToken cancellationToken)
    {
        var otherMemberSplitPercentageTotal = await _db.Members
            .Where(m => m.HomeId == command.HomeId)
            .Select(m => m.SplitPercentage)
            .SumAsync(cancellationToken);

        return otherMemberSplitPercentageTotal + command.Props.SplitPercentage > 1;
    }
}

public class UpdateMemberCommandValidator : AbstractValidator<UpdateMemberCommand>
{
    private readonly ApplicationDbContext _db;

    public UpdateMemberCommandValidator(ApplicationDbContext db)
    {
        _db = db;

        RuleFor(x => x.Props)
            .SetValidator(new MemberPropertiesValidator());

        RuleFor(x => x)
            .MustAsync(SplitPercentageCannotExceedMaximumAmongHomeMembers);
    }

    // TODO: Would it be better to just do this in the command handler?
    public async Task<bool> SplitPercentageCannotExceedMaximumAmongHomeMembers(UpdateMemberCommand command,
        CancellationToken cancellationToken)
    {
        var otherMemberSplitPercentageTotal = await _db.Members
            .Where(m => m.HomeId == command.HomeId && m.Id != command.MemberId)
            .Select(m => m.SplitPercentage)
            .SumAsync(cancellationToken);

        return otherMemberSplitPercentageTotal + command.Props.SplitPercentage > 1;
    }
}

public class MemberPropertiesValidator : AbstractValidator<MemberProperties>
{
    public MemberPropertiesValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty();

        RuleFor(x => x.SplitPercentage)
            .InclusiveBetween(0d, 1d);
    }
}

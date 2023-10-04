using FluentValidation;
using Spenses.Application.Features.Common.Validators;
using Spenses.Application.Features.Credits.Requests;
using Spenses.Application.Models.Credits;

namespace Spenses.Application.Features.Credits.Validators;

public class CreditsQueryValidator : AbstractValidator<CreditsQuery>
{
    public CreditsQueryValidator()
    {
        Include(new PagedQueryValidator<CreditDigest>());

        RuleFor(x => x.MinDate)
            .LessThanOrEqualTo(x => x.MaxDate)
            .When(x => x.MaxDate.HasValue);

        RuleFor(x => x.MaxDate)
            .GreaterThanOrEqualTo(x => x.MinDate)
            .When(x => x.MinDate.HasValue);
    }
}

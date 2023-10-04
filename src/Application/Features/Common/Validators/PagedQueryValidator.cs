using System.Reflection;
using FluentValidation;
using Hexagrams.Extensions.Common;
using Spenses.Application.Common.Query;
using Spenses.Application.Models.Common;

namespace Spenses.Application.Features.Common.Validators;

public class PagedQueryValidator<T> : AbstractValidator<PagedQuery<T>>
    where T : class
{
    public PagedQueryValidator()
    {
        RuleFor(x => x.PageNumber)
            .NotNull()
            .GreaterThanOrEqualTo(1);

        RuleFor(x => x.PageSize)
            .NotNull()
            .InclusiveBetween(1, 200);

        RuleFor(x => x.SortDirection)
            .NotNull().When(x => !string.IsNullOrEmpty(x.OrderBy))
            .IsInEnum();

        var orderablePropertyNames = typeof(T)
            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(p => p.CustomAttributes.Any(a => a.AttributeType == typeof(OrderableAttribute)))
            .Select(p => p.Name.ToLowerCamelCase());

        RuleFor(x => x.OrderBy)
            .NotEmpty()
            .When(x => x.SortDirection.HasValue);

        RuleFor(x => x.OrderBy)
            .Must(x => x is null || orderablePropertyNames.Contains(x.ToLowerCamelCase()))
            .When(x => !string.IsNullOrEmpty(x.OrderBy))
            .WithMessage(x =>
                $"'{x.OrderBy}' is not an orderable property. Valid options are: " +
                string.Join(", ", orderablePropertyNames));
    }
}

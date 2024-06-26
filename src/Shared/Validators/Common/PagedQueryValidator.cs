using System.Reflection;
using FluentValidation;
using Hexagrams.Extensions.Common;
using Spenses.Shared.Common.Query;
using Spenses.Shared.Models.Common;

namespace Spenses.Shared.Validators.Common;

public class PagedQueryValidator<T> : AbstractValidator<PagedQuery<T>>
    where T : class
{
    public PagedQueryValidator()
    {
        RuleFor(x => x.Skip)
            .GreaterThanOrEqualTo(0);

        RuleFor(x => x.Take)
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


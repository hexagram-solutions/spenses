using FluentValidation.TestHelper;
using Spenses.Application.Features.Common.Validators;
using Spenses.Shared.Common.Query;
using Spenses.Shared.Models.Common;

namespace Spenses.Application.Tests.Features.Common.Validators;

public abstract class PagedQueryValidatorTests<TModel> where TModel : class
{
    private readonly PagedQueryValidator<TModel> _validator = new();

    [Fact]
    public void Page_number_must_be_greater_than_0()
    {
        _validator.TestValidate(new PagedQuery<TModel> { Skip = -1 })
            .ShouldHaveValidationErrorFor(x => x.Skip);
    }

    [Fact]
    public void Page_size_must_be_in_valid_range()
    {
        var model = new PagedQuery<TModel> { Take = 0 };

        _validator.TestValidate(model)
            .ShouldHaveValidationErrorFor(x => x.Take);

        _validator.TestValidate(model with { Take = 201 })
            .ShouldHaveValidationErrorFor(x => x.Take);
    }

    [Fact]
    public void Sort_order_must_be_valid()
    {
        var model = new PagedQuery<TModel> { SortDirection = (SortDirection) (-1) };

        _validator.TestValidate(model)
            .ShouldHaveValidationErrorFor(x => x.SortDirection);

        _validator.TestValidate(model with { SortDirection = null })
            .ShouldNotHaveValidationErrorFor(x => x.SortDirection);

        _validator.TestValidate(model with { SortDirection = SortDirection.Desc })
            .ShouldNotHaveValidationErrorFor(x => x.SortDirection);
    }

    public static IEnumerable<object[]> OrderableProperties => typeof(TModel).GetProperties()
        .Where(pi => pi.CustomAttributes.Any(ca => ca.AttributeType == typeof(OrderableAttribute)))
        .Select(pi => new object[] { pi.Name });

    public static IEnumerable<object[]> NonOrderableProperties => typeof(TModel).GetProperties()
        .Where(pi => pi.CustomAttributes.All(ca => ca.AttributeType != typeof(OrderableAttribute)))
        .Select(pi => new object[] { pi.Name });

    [Theory]
    [MemberData(nameof(OrderableProperties))]
    public void Order_by_must_be_valid(string propertyName)
    {
        _validator.TestValidate(new PagedQuery<TModel> { OrderBy = propertyName })
            .ShouldNotHaveValidationErrorFor(x => x.OrderBy);
    }

    [Theory]
    [MemberData(nameof(NonOrderableProperties))]
    public void Non_orderable_properties_cannot_be_used_for_sorting(string propertyName)
    {
        _validator.TestValidate(new PagedQuery<TModel> { OrderBy = propertyName })
            .ShouldHaveValidationErrorFor(x => x.OrderBy);
    }

    [Fact]
    public void Sort_direction_and_order_by_must_be_set_if_either_are_set()
    {
        var model = new PagedQuery<TModel> { OrderBy = (string) OrderableProperties.First().First() };

        _validator.TestValidate(model)
            .ShouldHaveValidationErrorFor(x => x.SortDirection);

        _validator.TestValidate(model with { SortDirection = SortDirection.Asc, OrderBy = null })
            .ShouldHaveValidationErrorFor(x => x.OrderBy);

        _validator.TestValidate(model with
        {
            OrderBy = (string) OrderableProperties.First().First(),
            SortDirection = SortDirection.Asc
        }).ShouldNotHaveValidationErrorFor(x => x.OrderBy);
    }
}

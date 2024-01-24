using FluentValidation;

namespace Spenses.App.Infrastructure;

public static class ValidationExtensions
{

    // This method is very weird. It returns a validation function that is compatible with MudBlazor forms and provides
    // error messages with superior UX compared to the default fluent validation error messages.
    // See https://www.mudblazor.com/components/form#using-fluent-validation for examples.
    public static Func<object, string, Task<IEnumerable<string>>> ValidateValue<TValue>(
        this AbstractValidator<TValue> validator)
    {
        return async (model, property) =>
        {
            var result = await validator.ValidateAsync(
                ValidationContext<TValue>.CreateWithOptions((TValue) model,
                    x => x.IncludeProperties(property)));

            return result.IsValid ? [] : result.Errors.Select(e => e.ErrorMessage);
        };
    }
}

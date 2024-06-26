using FluentValidation;
using MediatR;
using Spenses.Application.Exceptions;

namespace Spenses.Application.Behaviors;

public class RequestValidationBehaviour<TRequest, TResponse>(IEnumerable<IValidator<TRequest>> validators)
    : IPipelineBehavior<TRequest, TResponse> where TRequest : notnull
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (!validators.Any())
            return await next();

        var context = new ValidationContext<TRequest>(request);

        var validationResults = await Task.WhenAll(
            validators.Select(v =>
                v.ValidateAsync(context, cancellationToken)));

        var failures = validationResults
            .Where(r => r.Errors.Count != 0)
            .SelectMany(r => r.Errors)
            .ToList();

        if (failures.Count == 0)
            return await next();

        // Remove first level of nested property names. For example, a validation error for property "Foo.Bar.Baz" will
        // be changed to a validation error for "Bar.Baz". This is done to present a friendly validation error message
        // that does not contain the property name of the MediatR request being validated.
        // TODO: This could be stupid. Had to do this because we can't add the right type params to the behavior to be able to select the property from the model.
        failures.ForEach(f =>
        {
            var propertyNamePath = f.PropertyName.Split('.');

            if (propertyNamePath.Length <= 1)
                return;

            var nestedMembersPath = propertyNamePath.Skip(1);
            var rootMemberName = propertyNamePath[0];

            f.PropertyName = string.Join('.', nestedMembersPath);

            var rootMemberNameMessageIndex = f.ErrorMessage.IndexOf(rootMemberName + " ", StringComparison.Ordinal);

            if (rootMemberNameMessageIndex >= 0)
                f.ErrorMessage = f.ErrorMessage.Remove(rootMemberNameMessageIndex, rootMemberName.Length + 1);
        });

        throw new InvalidRequestException(failures);
    }
}

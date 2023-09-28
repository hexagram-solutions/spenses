using System.Reflection;
using AutoMapper;
using AutoMapper.Configuration;

namespace Spenses.Application.Extensions;

public static class AutoMapperExtensions
{
    private const string TypeMapActionsPropertyName = "TypeMapActions";

    // TypeMapConfiguration.TypeMapActions is protected, so we can't use nameof() here.
    private static readonly PropertyInfo? TypeMapActionsProperty =
        typeof(TypeMapConfiguration).GetProperty(TypeMapActionsPropertyName, BindingFlags.NonPublic | BindingFlags.Instance);

    public static void ForAllOtherMembers<TSource, TDestination>(
        this IMappingExpression<TSource, TDestination> expression,
        Action<IMemberConfigurationExpression<TSource, TDestination, object>> memberOptions)
    {
        var typeMapConfiguration = (TypeMapConfiguration) expression;

        if (TypeMapActionsProperty?.GetValue(typeMapConfiguration) is not List<Action<TypeMap>> typeMapActionsValue)
        {
            throw new InvalidOperationException("Could not find property" +
                $"{typeof(TypeMapConfiguration).FullName}.{TypeMapActionsPropertyName} via reflection. Ensure the " +
                $"property still exists on {typeof(TypeMapConfiguration).FullName}.");
        }

        typeMapActionsValue.Add(typeMap =>
        {
            var destinationTypeDetails = typeMap.DestinationTypeDetails;

            foreach (var accessor in destinationTypeDetails.WriteAccessors.Where(m =>
                         typeMapConfiguration.GetDestinationMemberConfiguration(m) == null))
            {
                expression.ForMember(accessor.Name, memberOptions);
            }
        });
    }
}

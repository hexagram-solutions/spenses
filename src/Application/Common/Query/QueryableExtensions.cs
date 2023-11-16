using System.Linq.Expressions;
using System.Reflection;
using Spenses.Application.Models.Common;

namespace Spenses.Application.Common.Query;

public static class QueryableExtensions
{
    private static readonly MethodInfo QueryableOrderBy =
        ExpressionHelper.GetMethod<IQueryable<object>>(x => x.OrderBy(y => y));

    private static readonly MethodInfo QueryableOrderByDescending =
        ExpressionHelper.GetMethod<IQueryable<object>>(x => x.OrderByDescending(y => y));

    private static readonly MethodInfo QueryableThenBy =
        ExpressionHelper.GetMethod<IOrderedQueryable<object>>(x => x.ThenBy(y => y));

    private static readonly MethodInfo QueryableThenByDescending =
        ExpressionHelper.GetMethod<IOrderedQueryable<object>>(x => x.ThenByDescending(y => y));

    public static IQueryable<T> OrderBy<T>(this IQueryable<T> source, string[] path, SortDirection direction,
        bool isFirst)
    {
        const BindingFlags bindingFlags =
            BindingFlags.Instance | BindingFlags.FlattenHierarchy | BindingFlags.Public | BindingFlags.NonPublic;

        var currentType = source.ElementType;

        var parameterExpression = Expression.Parameter(currentType, "x");

        Expression memberAccess = parameterExpression;

        foreach (var part in path)
        {
            var propertyInfo = currentType.GetProperty(part, bindingFlags);

            if (propertyInfo == null)
            {
                throw new InvalidOperationException(
                    $"The property '{part}' was not found on type '{currentType.FullName}' when evaluating the path " +
                    $"{string.Join(".", path)}.");
            }

            memberAccess = Expression.MakeMemberAccess(memberAccess, propertyInfo);

            currentType = propertyInfo.PropertyType;
        }

        var keySelector = Expression.Lambda(memberAccess, parameterExpression);

        var methodInfo = isFirst
            ? direction != SortDirection.Desc ? QueryableOrderBy : QueryableOrderByDescending
            : direction != SortDirection.Desc
                ? QueryableThenBy
                : QueryableThenByDescending;

        methodInfo = methodInfo.MakeGenericMethod(source.ElementType, currentType);

        return (IOrderedQueryable<T>) source.Provider.CreateQuery(
            Expression.Call(null, methodInfo,
            [
                    source.Expression,
                Expression.Quote(keySelector)
            ]));
    }
}

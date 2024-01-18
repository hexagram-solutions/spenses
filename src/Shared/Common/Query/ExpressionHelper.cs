using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace Spenses.Shared.Common.Query;

// TODO: Move to shared lib
public static class ExpressionHelper
{
    public static PropertyInfo FindProperty(Type type, string memberName)
    {
        ArgumentNullException.ThrowIfNull(type);

        var property = type.GetProperty(memberName,
            BindingFlags.Instance | BindingFlags.FlattenHierarchy | BindingFlags.Public);

        if (property is null)
            throw new InvalidOperationException(
                $"The property '{memberName}' was not found on type '{type.FullName}'");

        return property;
    }

    public static Expression BuildMemberAccessForPath(Expression parameter, string path)
    {
        ArgumentNullException.ThrowIfNull(parameter);
        ArgumentNullException.ThrowIfNull(path);

        var result = parameter;

        if (path.Length == 0)
            return result;

        var memberNames = path.Split('.');

        foreach (var memberName in memberNames)
        {
            var property = FindProperty(result.Type, memberName);

            result = Expression.MakeMemberAccess(result, property);
        }

        return result;
    }

    public static MethodInfo GetMethod<T>(Expression<Action<T>> methodExpression)
    {
        var methodCall = (MethodCallExpression) methodExpression.Body;

        var result = methodCall.Method;

        if (result.IsGenericMethod)
            result = result.GetGenericMethodDefinition();

        return result;
    }

    public static MethodInfo GetMethod<T>(Expression<Func<T, object>> methodExpression)
    {
        var body = methodExpression.Body;

        while (body is UnaryExpression unaryExpression)
            body = unaryExpression.Operand;

        var methodCall = (MethodCallExpression) body;

        var result = methodCall.Method;

        if (result.IsGenericMethod)
            result = result.GetGenericMethodDefinition();

        return result;
    }

    public static Expression RemoveCaseConversionExpressions(Expression input)
    {
        while (input is MethodCallExpression methodCallExpression &&
               methodCallExpression.Method.DeclaringType == typeof(string))
        {
            switch (methodCallExpression.Method.Name)
            {
                case "ToLower":
                case "ToLowerInvariant":
                case "ToUpper":
                case "ToUpperInvariant":
                    input = methodCallExpression.Object!;
                    continue;

                default:
                    return input;
            }
        }

        return input;
    }

    public static string GetPath(LambdaExpression pathExpression)
    {
        ArgumentNullException.ThrowIfNull(pathExpression);

        return GetPathInternal(pathExpression.Body);
    }

    public static string GetPath<T>(Expression<Func<T, object>> pathExpression)
    {
        ArgumentNullException.ThrowIfNull(pathExpression);

        return GetPathInternal(pathExpression.Body);
    }

    private static string GetPathInternal(Expression expression)
    {
        var inLambdaExpression = false;

        var result = new StringBuilder();

        while (true)
        {
            switch (expression.NodeType)
            {
                case ExpressionType.Convert:
                case ExpressionType.ConvertChecked:
                case ExpressionType.Quote:
                case ExpressionType.TypeAs:
                    var unaryExpression = (UnaryExpression) expression;

                    expression = unaryExpression.Operand;

                    continue;

                case ExpressionType.Lambda:
                {
                    if (inLambdaExpression)
                        throw new InvalidOperationException("Unable to extract path from nested lambda expression");

                    inLambdaExpression = true;

                    var lambdaExpression = (LambdaExpression) expression;

                    expression = lambdaExpression.Body;

                    continue;
                }

                case ExpressionType.Call:
                {
                    var methodCallExpression = (MethodCallExpression) expression;

                    if (methodCallExpression.Method.DeclaringType == typeof(Enumerable) &&
                        methodCallExpression.Method.Name == nameof(Enumerable.Select) ||
                        methodCallExpression.Method.Name == nameof(Enumerable.SelectMany))
                    {
                        if (result.Length > 0)
                            result.Insert(0, '.');

                        var lambda = (LambdaExpression) methodCallExpression.Arguments[1];

                        result.Insert(0, GetPathInternal(lambda.Body));

                        expression = methodCallExpression.Arguments[0];

                        continue;
                    }

                    goto default;
                }

                case ExpressionType.MemberAccess:
                {
                    var memberExpression = (MemberExpression) expression;

                    if (result.Length > 0)
                        result.Insert(0, '.');

                    result.Insert(0, memberExpression.Member.Name);

                    expression = memberExpression.Expression!;

                    continue;
                }

                case ExpressionType.Parameter:
                    return result.ToString();

                default:
                    throw new InvalidOperationException("The expression could not be converted to a path.");
            }
        }
    }

    public static object? Evaluate(Expression expression)
    {
        ArgumentNullException.ThrowIfNull(expression);

        if (!IsParameterless(expression))
            throw new ArgumentException("expression can not contain a ParameterExpression.", nameof(expression));

        if (IsNullConstant(expression))
            return null;

        expression = Expression.Convert(expression, typeof(object));

        var lambda = Expression.Lambda<Func<object>>(expression).Compile();

        return lambda();
    }

    private static bool IsNullConstant(Expression expression)
    {
        while (expression is UnaryExpression unaryExpression)
        {
            if (unaryExpression.NodeType != ExpressionType.Convert &&
                unaryExpression.NodeType != ExpressionType.ConvertChecked)
                break;

            expression = unaryExpression.Operand;
        }

        return expression is ConstantExpression constant && constant.Value == null;
    }

    public static bool IsParameterless(Expression expression)
    {
        var visitor = new ParameterExpressionVisitor();

        visitor.Visit(expression);

        return visitor.Parameters.Count == 0;
    }

    public static ParameterExpression[] GetParameters(Expression expression)
    {
        var visitor = new ParameterExpressionVisitor();

        visitor.Visit(expression);

        return [.. visitor.Parameters];
    }

    public static LambdaExpression ExtractLambdaExpression(Expression expression)
    {
        while (true)
        {
            switch (expression.NodeType)
            {
                case ExpressionType.Convert:
                case ExpressionType.ConvertChecked:
                case ExpressionType.Quote:
                case ExpressionType.TypeAs:
                    var unaryExpression = (UnaryExpression) expression;
                    expression = unaryExpression.Operand;
                    break;

                case ExpressionType.Lambda:
                    return (LambdaExpression) expression;

                default:
                    throw new InvalidOperationException($"Unable to extract a LambdaExpression from {expression}.");
            }
        }
    }

    private sealed class ParameterExpressionVisitor : ExpressionVisitor
    {
        public HashSet<ParameterExpression> Parameters { get; } = [];

        protected override Expression VisitParameter(ParameterExpression node)
        {
            Parameters.Add(node);

            return base.VisitParameter(node);
        }
    }
}

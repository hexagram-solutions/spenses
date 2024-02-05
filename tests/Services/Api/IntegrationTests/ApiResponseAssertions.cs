using System.Linq.Expressions;
using System.Reflection;
using Hexagrams.Extensions.Common.Serialization;
using Microsoft.AspNetCore.Mvc;
using Refit;

namespace Spenses.Api.IntegrationTests;

public class ApiResponseAssertions<TContent>(IApiResponse subject)
{
    public AndConstraint<ApiResponseAssertions<TContent>> HaveValidationErrorFor(string propertyName)
    {
        var validationProblems = subject.Error?.Content?.FromJson<ValidationProblemDetails>();

        validationProblems.Should().NotBeNull();

        validationProblems!.Errors.Should().ContainKey(propertyName);

        return new AndConstraint<ApiResponseAssertions<TContent>>(this);
    }

    public AndConstraint<ApiResponseAssertions<TContent>> HaveValidationErrorFor<TProperty>(
        Expression<Func<TContent, TProperty>> selector)
    {
        LambdaExpression lambda = selector;

        var memberExpression = lambda.Body is UnaryExpression expression
            ? (MemberExpression) expression.Operand
            : (MemberExpression) lambda.Body;

        var propertyInfo = (PropertyInfo) memberExpression.Member;

        return HaveValidationErrorFor(propertyInfo.Name);
    }
}

public static class ApiResponseAssertionsExtensions
{
    public static ApiResponseAssertions<TContent> Should<TContent>(this IApiResponse<TContent> apiResponse)
    {
        return new ApiResponseAssertions<TContent>(apiResponse);
    }
}

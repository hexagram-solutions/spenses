using Refit;
using Spenses.Application.Models.Expenses;
using Spenses.Application.Models.Members;
using Spenses.Client.Http;

namespace Spenses.Api.IntegrationTests.Expenses;

[Collection(WebApplicationCollection.CollectionName)]
public partial class ExpensesIntegrationTests(WebApplicationFixture<Program> fixture)
{
    private readonly IHomesApi _homes = RestService.For<IHomesApi>(fixture.WebApplicationFactory.CreateClient());

    private readonly IExpensesApi _expenses =
        RestService.For<IExpensesApi>(fixture.WebApplicationFactory.CreateClient(),
            new RefitSettings { CollectionFormat = CollectionFormat.Multi });

    private readonly IExpenseCategoriesApi _expenseCategories =
        RestService.For<IExpenseCategoriesApi>(fixture.WebApplicationFactory.CreateClient(),
            new RefitSettings { CollectionFormat = CollectionFormat.Multi });

    private ExpenseProperties GetValidExpenseProperties(IReadOnlyCollection<Member> members, Guid categoryId)
    {
        var amount = 99.99m;

        var properties = new ExpenseProperties
        {
            Note = "Foo",
            Amount = amount,
            Date = DateOnly.FromDateTime(DateTime.UtcNow),
            Tags = ["groceries"],
            CategoryId = categoryId,
            PaidByMemberId = members.First().Id,
            ExpenseShares = members.Select(m => new ExpenseShareProperties
            {
                OwedByMemberId = m.Id,
                OwedAmount = decimal.Round(decimal.Divide(amount, members.Count), 2)
            }).ToArray()
        };

        return properties;
    }
}

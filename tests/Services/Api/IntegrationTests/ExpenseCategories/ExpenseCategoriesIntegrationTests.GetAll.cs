using System.Net;

namespace Spenses.Api.IntegrationTests.ExpenseCategories;

public partial class ExpenseCategoriesIntegrationTests
{
    [Fact]
    public async Task Get_expense_categories_yields_categories_in_order()
    {
        var resp = await _homes.GetHomes();

        var homeId = (await _homes.GetHomes()).Content!.First().Id;

        var expenseCategories = (await _categories.GetExpenseCategories(homeId)).Content!;

        expenseCategories.Should()
            .BeInDescendingOrder(x => x.IsDefault)
            .And.ThenBeInAscendingOrder(x => x.Name);
    }

    [Fact]
    public async Task Get_expense_categories_with_invalid_identifiers_yields_not_found()
    {
        var homeNotFoundResult = await _categories.GetExpenseCategories(Guid.NewGuid());

        homeNotFoundResult.Error!.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}

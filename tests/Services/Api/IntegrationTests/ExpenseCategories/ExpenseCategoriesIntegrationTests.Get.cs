using System.Net;

namespace Spenses.Api.IntegrationTests.ExpenseCategories;

public partial class ExpenseCategoriesIntegrationTests
{
    [Fact]
    public async Task Get_expense_category_with_invalid_identifiers_yields_not_found()
    {
        var homeId = (await _homes.GetHomes()).Content!.First().Id;

        var categoryId = (await _categories.GetExpenseCategories(homeId)).Content!.First().Id;

        var homeNotFoundResult = await _categories.GetExpenseCategory(Guid.NewGuid(), categoryId);

        homeNotFoundResult.Error!.StatusCode.Should().Be(HttpStatusCode.NotFound);

        var categoryNotFoundResult = await _categories.GetExpenseCategory(homeId, Guid.NewGuid());

        categoryNotFoundResult.Error!.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}

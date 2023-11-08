using System.Net;

namespace Spenses.Api.IntegrationTests.ExpenseCategories;

public partial class ExpenseCategoriesIntegrationTests
{
    [Fact]
    public async Task Get_expense_categories_with_invalid_identifiers_yields_not_found()
    {
        var homeNotFoundResult = await _categories.GetExpenseCategories(Guid.NewGuid());

        homeNotFoundResult.Error!.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}

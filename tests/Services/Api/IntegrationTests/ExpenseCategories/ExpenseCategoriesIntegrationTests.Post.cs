using System.Net;
using Spenses.Application.Models.ExpenseCategories;

namespace Spenses.Api.IntegrationTests.ExpenseCategories;
public partial class ExpenseCategoriesIntegrationTests
{
    [Fact]
    public async Task Post_expense_category_creates_category()
    {
        var home = (await _homes.GetHomes()).Content!.First();

        var properties = new ExpenseCategoryProperties
        {
            Name = $"Gubbins_{Guid.NewGuid()}",
            Description = "Provisions for the coming war"
        };

        var createdCategoryResponse = await _categories.PostExpenseCategory(home.Id, properties);

        createdCategoryResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        var createdCategory = createdCategoryResponse.Content;

        createdCategory.Should().BeEquivalentTo(properties, opts =>
            opts.ExcludingNestedObjects()
                .ExcludingMissingMembers());

        var members = (await _categories.GetExpenseCategories(home.Id)).Content;
        members.Should().ContainEquivalentOf(createdCategory);

        await _categories.DeleteExpenseCategory(home.Id, createdCategory!.Id);
    }

    [Fact]
    public async Task Post_expense_category_with_invalid_identifiers_yields_not_found()
    {
        var properties = new ExpenseCategoryProperties
        {
            Name = $"Gubbins_{Guid.NewGuid()}",
            Description = "Provisions for the coming war"
        };

        var homeNotFoundResult = await _categories.PostExpenseCategory(Guid.NewGuid(), properties);

        homeNotFoundResult.Error!.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}

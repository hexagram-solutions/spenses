using System.Net;
using Spenses.Application.Models.ExpenseCategories;

namespace Spenses.Api.IntegrationTests.ExpenseCategories;

public partial class ExpenseCategoriesIntegrationTests
{
    [Fact]
    public async Task Put_expense_category_updates_category()
    {
        var home = (await _homes.GetHomes()).Content!.First();

        var categories = (await _categories.GetExpenseCategories(home.Id)).Content!;

        var categoryId = categories.First(x => !x.IsDefault).Id;

        var properties = new ExpenseCategoryProperties
        {
            Name = $"Gubbins_{Guid.NewGuid()}",
            Description = "Provisions for the coming war"
        };

        var updatedCategoryResponse = await _categories.PutExpenseCategory(home.Id, categoryId, properties);

        updatedCategoryResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var updatedCategory = updatedCategoryResponse.Content;

        updatedCategory.Should().BeEquivalentTo(properties, opts =>
            opts.ExcludingNestedObjects()
                .ExcludingMissingMembers());

        var fetchedCategory = (await _categories.GetExpenseCategory(home.Id, updatedCategory!.Id)).Content;
        fetchedCategory.Should().BeEquivalentTo(updatedCategory);
    }

    [Fact]
    public async Task Put_expense_category_with_invalid_identifiers_yields_not_found()
    {
        var homeId = (await _homes.GetHomes()).Content!.First().Id;

        var categories = (await _categories.GetExpenseCategories(homeId)).Content!;

        var categoryId = categories.First(x => !x.IsDefault).Id;

        var properties = new ExpenseCategoryProperties
        {
            Name = $"Gubbins_{Guid.NewGuid()}",
            Description = "Provisions for the coming war"
        };

        var homeNotFoundResult = await _categories.PutExpenseCategory(Guid.NewGuid(), categoryId, properties);

        homeNotFoundResult.Error!.StatusCode.Should().Be(HttpStatusCode.NotFound);

        var categoryNotFoundResult = await _categories.PutExpenseCategory(homeId, Guid.NewGuid(), properties);

        categoryNotFoundResult.Error!.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Put_expense_category_yields_error_when_updating_default_category()
    {
        var homeId = (await _homes.GetHomes()).Content!.First().Id;

        var categories = (await _categories.GetExpenseCategories(homeId)).Content!;

        var categoryId = categories.First(x => x.IsDefault).Id;

        var homeNotFoundResult = await _categories.DeleteExpenseCategory(homeId, categoryId);

        homeNotFoundResult.Error!.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}

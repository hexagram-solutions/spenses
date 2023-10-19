using System.Net;
using Refit;
using Spenses.Application.Models.ExpenseCategories;
using Spenses.Client.Http;

namespace Spenses.Api.IntegrationTests.ExpenseCategories;

[Collection(WebApplicationCollection.CollectionName)]
public class ExpenseCategoriesIntegrationTests
{
    private readonly IExpenseCategoriesApi _categories;
    private readonly IHomesApi _homes;

    public ExpenseCategoriesIntegrationTests(WebApplicationFixture<Program> fixture)
    {
        _categories = RestService.For<IExpenseCategoriesApi>(fixture.WebApplicationFactory.CreateClient());
        _homes = RestService.For<IHomesApi>(fixture.WebApplicationFactory.CreateClient());
    }

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
    public async Task Put_expense_category_updates_category()
    {
        var home = (await _homes.GetHomes()).Content!.First();

        var category = (await _categories.GetExpenseCategories(home.Id)).Content!.First();

        var properties = new ExpenseCategoryProperties
        {
            Name = $"Gubbins_{Guid.NewGuid()}",
            Description = "Provisions for the coming war"
        };

        var updatedCategoryResponse = await _categories.PutExpenseCategory(home.Id, category.Id, properties);

        updatedCategoryResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var updatedCategory = updatedCategoryResponse.Content;

        updatedCategory.Should().BeEquivalentTo(properties, opts =>
            opts.ExcludingNestedObjects()
                .ExcludingMissingMembers());

        var fetchedCategory = (await _categories.GetExpenseCategory(home.Id, updatedCategory!.Id)).Content;
        fetchedCategory.Should().BeEquivalentTo(updatedCategory);
    }
}

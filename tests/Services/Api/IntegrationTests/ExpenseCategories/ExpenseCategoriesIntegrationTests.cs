using FluentAssertions.Common;
using Spenses.Client.Http;

namespace Spenses.Api.IntegrationTests.ExpenseCategories;

// the base class will get created once for every test because it is not a class fixture, it has to do setup (constructor)
// and teardown (dispose) for each test
public partial class ExpenseCategoriesIntegrationTests : IdentityIntegrationTestBase
{
    private IExpenseCategoriesApi _categories;
    private IHomesApi _homes;

    public ExpenseCategoriesIntegrationTests(DatabaseFixture databaseFixture, AuthenticationFixture authFixture)
        : base(databaseFixture, authFixture)
    {
    }

    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();

        _categories = CreateApiClient<IExpenseCategoriesApi>();
        _homes = CreateApiClient<IHomesApi>();
    }
}

using Microsoft.AspNetCore.Components;
using Spenses.Shared.Models.ExpenseCategories;

namespace Spenses.Client.Web.Components.ExpenseCategories;

public partial class ExpenseCategoryForm
{
    [Parameter]
    public ExpenseCategoryProperties ExpenseCategory { get; set; } = new();

    public Validations Validations { get; set; } = null!;
}

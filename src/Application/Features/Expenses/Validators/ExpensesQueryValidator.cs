using FluentValidation;
using Spenses.Application.Features.Common.Validators;
using Spenses.Application.Features.Expenses.Requests;
using Spenses.Application.Models.Expenses;

namespace Spenses.Application.Features.Expenses.Validators;

public class ExpensesQueryValidator : AbstractValidator<ExpensesQuery>
{
    public ExpensesQueryValidator()
    {
        Include(new PagedQueryValidator<ExpenseDigest>());
    }
}

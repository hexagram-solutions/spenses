using Microsoft.AspNetCore.Identity;

namespace Spenses.Resources.Relational.Models;

public class ApplicationUser : IdentityUser<Guid>
{
    [PersonalData]
    public string DisplayName { get; set; } = null!;

    public string AvatarUrl { get; set; } = null!;

    public ICollection<Payment> CreatedPayments { get; set; } = [];

    public ICollection<Payment> ModifiedPayments { get; set; } = [];

    public ICollection<ExpenseCategory> CreatedExpenseCategories { get; set; } = [];

    public ICollection<ExpenseCategory> ModifiedExpenseCategories { get; set; } = [];

    public ICollection<Expense> CreatedExpenses { get; set; } = [];

    public ICollection<Expense> ModifiedExpenses { get; set; } = [];

    public ICollection<Home> CreatedHomes { get; set; } = [];

    public ICollection<Home> ModifiedHomes { get; set; } = [];

    public ICollection<Member> CreatedMembers { get; set; } = [];

    public ICollection<Member> ModifiedMembers { get; set; } = [];

    public ICollection<Invitation> CreatedInvitations { get; set; } = [];

    public ICollection<Invitation> ModifiedInvitations { get; set; } = [];
}

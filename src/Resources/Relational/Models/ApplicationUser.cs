using Microsoft.AspNetCore.Identity;

namespace Spenses.Resources.Relational.Models;

public class ApplicationUser : IdentityUser
{
    public ICollection<Payment> CreatedPayments { get; set; } = new HashSet<Payment>();

    public ICollection<Payment> ModifiedPayments { get; set; } = new HashSet<Payment>();

    public ICollection<ExpenseCategory> CreatedExpenseCategories { get; set; } = new HashSet<ExpenseCategory>();

    public ICollection<ExpenseCategory> ModifiedExpenseCategories { get; set; } = new HashSet<ExpenseCategory>();

    public ICollection<Expense> CreatedExpenses { get; set; } = new HashSet<Expense>();

    public ICollection<Expense> ModifiedExpenses { get; set; } = new HashSet<Expense>();

    public ICollection<Home> CreatedHomes { get; set; } = new HashSet<Home>();

    public ICollection<Home> ModifiedHomes { get; set; } = new HashSet<Home>();

    public ICollection<Member> CreatedMembers { get; set; } = new HashSet<Member>();

    public ICollection<Member> ModifiedMembers { get; set; } = new HashSet<Member>();
}

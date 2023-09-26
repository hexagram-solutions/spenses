using Refit;
using Spenses.Application.Models;

namespace Spenses.Client.Http;

public interface IHomesApi
{
    [Post("/homes")]
    Task<Home> PostHome(HomeProperties props);

    [Get("/homes")]
    Task<IEnumerable<Home>> GetHomes();

    [Get("/homes/{homeId}")]
    Task<Home> GetHome(Guid homeId);

    [Put("/homes/{homeId}")]
    Task<Home> PutHome(Guid homeId, HomeProperties props);

    [Delete("/homes/{homeId}")]
    Task DeleteHome(Guid homeId);
}

public interface IHomeMembersApi
{
    [Post("/homes/{homeId}/members")]
    Task<Member> PostHomeMember(Guid homeId, MemberProperties props);

    [Get("/homes/{homeId}/members")]
    Task<IEnumerable<Member>> GetHomeMembers(Guid homeId);

    [Get("/homes/{homeId}/members/{memberId}")]
    Task<Member> GetHomeMember(Guid homeId, Guid memberId);

    [Put("/homes/{homeId}/members/{memberId}")]
    Task<Member> PutHomeMember(Guid homeId, Guid memberId, MemberProperties props);

    [Delete("/homes/{homeId}/members/{memberId}")]
    Task DeleteHomeMember(Guid homeId, Guid memberId);
}

public interface IHomeExpensesApi
{
    [Post("/homes/{homeId}/expenses")]
    Task<Expense> PostHomeExpense(Guid homeId, ExpenseProperties props);

    [Get("/homes/{homeId}/expenses/{expenseId}")]
    Task<Expense> GetHomeExpense(Guid homeId, Guid expenseId);

    [Put("/homes/{homeId}/expenses/{expenseId}")]
    Task<Expense> PutHomeExpense(Guid homeId, Guid expenseId, ExpenseProperties props);

    [Delete("/homes/{homeId}/expenses/{expenseId}")]
    Task<Expense> DeleteHomeExpense(Guid homeId, Guid expenseId);
}


using Refit;
using Spenses.Application.Models;

namespace Spenses.Client;

public interface IHomesApi
{
    [Post("/homes")]
    Task<Home> PostHome(HomeProperties props);

    [Get("/homes")]
    Task<Home[]> GetHomes();

    [Get("/homes/{homeId}")]
    Task<Home> GetHome(Guid homeId);

    [Get("/homes/{homeId}")]
    Task<Home> PutHome(Guid homeId, HomeProperties props);

    [Post("/homes/{homeId}/members")]
    Task<Member> PostHomeMember(Guid homeId, MemberProperties props);

    [Get("/homes/{homeId}/members/{memberId}")]
    Task<Member> GetHomeMember(Guid homeId, Guid memberId);

    [Put("/homes/{homeId}/members/{memberId}")]
    Task<Member> PutHomeMember(Guid homeId, Guid memberId, MemberProperties props);
}

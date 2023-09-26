using Refit;
using Spenses.Application.Models;

namespace Spenses.Client.Http;

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

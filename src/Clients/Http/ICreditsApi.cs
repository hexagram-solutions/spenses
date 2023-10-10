using Refit;
using Spenses.Application.Models.Common;
using Spenses.Application.Models.Credits;

namespace Spenses.Client.Http;

public interface ICreditsApi
{
    [Post("/homes/{homeId}/credits")]
    Task<ApiResponse<Credit>> PostCredit(Guid homeId, CreditProperties props);

    [Get("/homes/{homeId}/credits")]
    Task<ApiResponse<PagedResult<CreditDigest>>> GetCredits(Guid homeId, [Query] FilteredCreditsQuery query);

    [Get("/homes/{homeId}/credits/{creditId}")]
    Task<ApiResponse<Credit>> GetCredit(Guid homeId, Guid creditId);

    [Put("/homes/{homeId}/credits/{creditId}")]
    Task<ApiResponse<Credit>> PutCredit(Guid homeId, Guid creditId, CreditProperties props);

    [Delete("/homes/{homeId}/credits/{creditId}")]
    Task DeleteCredit(Guid homeId, Guid creditId);
}

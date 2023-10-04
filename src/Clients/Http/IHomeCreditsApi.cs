using Refit;
using Spenses.Application.Models.Credits;

namespace Spenses.Client.Http;

public interface IHomeCreditsApi
{
    [Post("/homes/{homeId}/credits")]
    Task<ApiResponse<Credit>> PostHomeCredit(Guid homeId, CreditProperties props);

    [Get("/homes/{homeId}/credits")]
    Task<ApiResponse<IEnumerable<Credit>>> GetHomeCredits(Guid homeId);

    [Get("/homes/{homeId}/credits/{creditId}")]
    Task<ApiResponse<Credit>> GetHomeCredit(Guid homeId, Guid creditId);

    [Put("/homes/{homeId}/credits/{creditId}")]
    Task<ApiResponse<Credit>> PutHomeCredit(Guid homeId, Guid creditId, CreditProperties props);

    [Delete("/homes/{homeId}/credits/{creditId}")]
    Task DeleteHomeCredit(Guid homeId, Guid creditId);
}

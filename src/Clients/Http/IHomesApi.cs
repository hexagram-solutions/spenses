using Refit;
using Spenses.Application.Models;

namespace Spenses.Client.Http;

public interface IHomesApi
{
    [Post("/homes")]
    Task<ApiResponse<Home>> PostHome(HomeProperties props);

    [Get("/homes")]
    Task<ApiResponse<IEnumerable<Home>>> GetHomes();

    [Get("/homes/{homeId}")]
    Task<ApiResponse<Home>> GetHome(Guid homeId);

    [Put("/homes/{homeId}")]
    Task<ApiResponse<Home>> PutHome(Guid homeId, HomeProperties props);

    [Delete("/homes/{homeId}")]
    Task DeleteHome(Guid homeId);
}

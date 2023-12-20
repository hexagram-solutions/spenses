using Refit;
using Spenses.Application.Models.Authentication;

namespace Spenses.Client.Http;

public interface IMeApi
{
    [Get("/me/info")]
    public Task<IApiResponse<CurrentUser>> GetMe();

    [Put("/me/info")]
    public Task<IApiResponse<CurrentUser>> UpdateMe();
}

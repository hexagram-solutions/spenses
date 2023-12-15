using Microsoft.AspNetCore.Components;
using Spenses.Client.Http;

namespace Spenses.Web.Client.Components.Pages;

public partial class Homes
{
    [Inject]
    private IHomesApi HomesApi { get; set; } = null!;

    private Application.Models.Homes.Home[] HomesItems { get; set; } = [];

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();

        var response = await HomesApi.GetHomes();

        HomesItems = response.Content!.ToArray();
    }
}

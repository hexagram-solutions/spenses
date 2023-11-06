using Microsoft.AspNetCore.Components;
using Spenses.Application.Models.Homes;
using Spenses.Client.Web.Features.Homes;

namespace Spenses.Client.Web.Components.Homes;

public partial class CreateHomeModal
{
    [Inject]
    private IModalService ModalService { get; init; } = null!;

    public HomeProperties Home { get; set; } = new();

    public HomeForm HomeFormRef { get; set; } = null!;

    private HomeState HomeState => GetState<HomeState>();

    private Task Close()
    {
        return ModalService.Hide();
    }

    private async Task Save()
    {
        if (!await HomeFormRef.Validations.ValidateAll())
            return;

        await Mediator.Send(new HomeState.HomeCreated(Home));

        await Close();
    }
}

using Fluxor;
using Microsoft.AspNetCore.Components;
using Spenses.Application.Models.Homes;
using Spenses.Client.Web.Store.Homes;

namespace Spenses.Client.Web.Components.Homes;

public partial class CreateHomeModal
{
    [Inject]
    private IState<HomesState> HomesState { get; set; } = null!;

    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;

    [Inject]
    private IModalService ModalService { get; init; } = null!;

    public HomeProperties Home { get; set; } = new();

    public HomeForm HomeFormRef { get; set; } = null!;

    private Task Close()
    {
        return ModalService.Hide();
    }

    private async Task Save()
    {
        if (!await HomeFormRef.Validations.ValidateAll())
            return;

        Dispatcher.Dispatch(new HomeCreatedAction(Home));

        await Close();
    }
}

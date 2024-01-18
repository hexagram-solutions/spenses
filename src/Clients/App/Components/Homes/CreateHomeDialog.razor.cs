using Fluxor;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Morris.Blazor.Validation.Extensions;
using MudBlazor;
using Spenses.App.Store.Homes;
using Spenses.Shared.Models.Homes;

namespace Spenses.App.Components.Homes;

public partial class CreateHomeDialog
{
    [CascadingParameter]
    private MudDialogInstance Dialog { get; set; } = null!;

    [Inject]
    private IState<HomesState> HomesState { get; set; } = null!;

    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;

    public HomeProperties Home { get; set; } = new();

    private void Close()
    {
        Dialog.Cancel();
    }

    private void Save(EditContext editContext)
    {
        if (!editContext.ValidateObjectTree())
            return;

        Dispatcher.Dispatch(new HomeCreatedAction(Home));
    }
}

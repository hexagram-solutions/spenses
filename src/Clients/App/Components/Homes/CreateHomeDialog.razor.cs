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

    [Inject]
    private IDialogService DialogService { get; init; } = null!;

    public HomeProperties Home { get; set; } = new();

    public HomeForm HomeFormRef { get; set; } = null!;

    private void Close()
    {
        Dialog.Cancel();
    }

    private void Save(EditContext editContext)
    {
        if (!editContext.ValidateObjectTree())
            return;

        Dispatcher.Dispatch(new HomeCreatedAction(Home));

        Close();
    }
    private object OnTransformModel(object model)
    {
        if (model is Home home)
        {
            return new HomeProperties
            {
                Name = home.Name,
                Description = home.Description
            };
        }

        return model;
    }
}

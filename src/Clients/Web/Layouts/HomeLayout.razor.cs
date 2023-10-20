using Microsoft.AspNetCore.Components;

namespace Spenses.Client.Web.Layouts;

public partial class HomeLayout : LayoutComponentBase
{
    [CascadingParameter]
    public Guid HomeId { get; set; }

    protected override void OnParametersSet()
    {
        if ((Body?.Target as RouteView)?.RouteData.RouteValues.TryGetValue("homeId", out var homeId) == true)
        {
            HomeId = (Guid) homeId;
        }
    }
}

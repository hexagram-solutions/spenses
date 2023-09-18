using Nuke.Common;

partial class Build
{
    [Secret]
    [Parameter]
    readonly string SqlServerConnectionString =
        "Server=.;Database=Spenses;Trusted_Connection=True;Encrypt=False;";
}

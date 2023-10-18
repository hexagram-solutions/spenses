$project = "$PSScriptRoot\..\src\Resources\Relational\Spenses.Resources.Relational.csproj"

dotnet ef database drop --project $project
$projectDirectory = Split-Path -parent $project
Remove-Item "$projectDirectory\Migrations" -Recurse
dotnet ef migrations add Initial --project $project
dotnet ef database update --project $project

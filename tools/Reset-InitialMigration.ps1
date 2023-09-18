$project = "$PSScriptRoot\..\src\Resources\Relational\Spenses.Resources.Relational.csproj"

dotnet ef database drop --project $project
dotnet ef migrations remove --project $project
dotnet ef migrations add Initial --project $project
dotnet ef database update --project $project

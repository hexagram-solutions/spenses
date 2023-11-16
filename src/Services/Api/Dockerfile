#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["src/Services/Api/Spenses.Api.csproj", "src/Services/Api/"]
COPY ["src/Application/Spenses.Application.csproj", "src/Application/"]
COPY ["src/Resources/Relational/Spenses.Resources.Relational.csproj", "src/Resources/Relational/"]
COPY ["src/Utilities/Security/Spenses.Utilities.Security.csproj", "src/Utilities/Security/"]
RUN dotnet restore "src/Services/Api/Spenses.Api.csproj"
COPY . .
WORKDIR "/src/src/Services/Api"
RUN dotnet build "Spenses.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Spenses.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Spenses.Api.dll"]

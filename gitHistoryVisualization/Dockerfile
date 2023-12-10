FROM mcr.microsoft.com/dotnet/runtime:7.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["git-history-visualization/git-history-visualization.csproj", "git-history-visualization/"]
RUN dotnet restore "git-history-visualization/git-history-visualization.csproj"
COPY . .
WORKDIR "/src/git-history-visualization"
RUN dotnet build "git-history-visualization.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "git-history-visualization.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "git-history-visualization.dll"]

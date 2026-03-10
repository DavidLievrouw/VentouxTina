FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /repo

COPY src/Directory.Packages.props src/
COPY src/Directory.Build.props src/
COPY src/Directory.Build.targets src/
COPY src/version.txt src/
COPY src/VentouxTina.Web/VentouxTina.Web.csproj src/VentouxTina.Web/
RUN dotnet restore src/VentouxTina.Web/VentouxTina.Web.csproj

COPY src/ src/
RUN dotnet publish src/VentouxTina.Web/VentouxTina.Web.csproj \
    -c Release \
    -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:10.0-noble-chiseled AS runtime
WORKDIR /app
COPY --from=build /app/publish .

USER $APP_UID
EXPOSE 8080
ENTRYPOINT ["dotnet", "VentouxTina.Web.dll"]

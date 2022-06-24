# https://hub.docker.com/_/microsoft-dotnet-core
FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build
WORKDIR /source

# copy csproj and restore as distinct layers
COPY *.sln .
COPY MTracking.BLL/*.csproj MTracking.BLL/
COPY MTracking.Core/*.csproj MTracking.Core/
COPY MTracking.DAL/*.csproj  MTracking.DAL/
COPY MTracking.API/*.csproj MTracking.API/

RUN dotnet restore

# copy everything else and build app
COPY MTracking.BLL/ MTracking.BLL/
COPY MTracking.Core/ MTracking.Core/
COPY MTracking.DAL/  MTracking.DAL/
COPY MTracking.API/ MTracking.API/

RUN dotnet publish -c release -o /app  --no-restore

# final stage/image
FROM mcr.microsoft.com/dotnet/core/aspnet:3.1
WORKDIR /app
COPY --from=build /app ./
ENTRYPOINT ["dotnet", "MTracking.API.dll"]

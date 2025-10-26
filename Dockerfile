# Build Stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
ENV BuildingInsideDocker=true
WORKDIR /src

COPY . .

RUN dotnet restore "ShortURLCore.sln" --disable-parallel
RUN dotnet publish "ShortURLCore.Web/ShortURLCore.Web.csproj" -c Release -o /app/publish --no-restore

# Server Stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app

ENV TZ=America/Managua
ENV GENERIC_TIMEZONE=America/Managua

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "ShortURLCore.Web.dll"]

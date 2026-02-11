FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env
WORKDIR /app
COPY . ./

FROM build-env AS build
ENV ConnectionStrings__CSService="Data Source=crime-scene.db"
RUN dotnet restore
RUN dotnet build /app/src/CSService.Migrations/CSService.Migrations.csproj -c Release -o /app/migration

FROM build AS publish
RUN dotnet publish /app/src/CSService.API/CSService.API.csproj -c Release -o /app/out

FROM publish as migrating
WORKDIR /app
RUN dotnet /app/migration/CSService.Migrations.dll

FROM mcr.microsoft.com/dotnet/aspnet:8.0
ENV Model="/app/vosk-model-ru-0.42"
WORKDIR /app
COPY --from=build /app/vosk-model-ru-0.42 ./vosk-model-ru-0.42
WORKDIR /app/out
COPY --from=migrating /app/crime-scene.db .
COPY --from=migrating /app/out .
ENTRYPOINT ["dotnet", "CSService.API.dll"]
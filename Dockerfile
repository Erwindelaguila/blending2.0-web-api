FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY . .
RUN dotnet restore ./blending2.0-web-api.csproj
RUN dotnet build ./blending2.0-web-api.csproj -c Release -o /app/build

FROM build AS publish
RUN dotnet publish ./blending2.0-web-api.csproj -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "blending2.0-web-api.dll"]

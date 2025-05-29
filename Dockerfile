FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY . .
RUN dotnet restore ./blending_web_api.csproj
RUN dotnet build ./blending_web_api.csproj -c Release -o /app/build

FROM build AS publish
RUN dotnet publish ./blending_web_api.csproj -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "blending_web_api.dll"]

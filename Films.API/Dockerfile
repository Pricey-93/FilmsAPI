FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Films.API/Films.API.csproj", "Films.API/"]
COPY ["Films.Application/Films.Application.csproj", "Films.Application/"]
COPY ["Films.Contracts/Films.Contracts.csproj", "Films.Contracts/"]
RUN dotnet restore "Films.API/Films.API.csproj"
COPY . .
WORKDIR "/src/Films.API"
RUN dotnet build "Films.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Films.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Films.API.dll"]

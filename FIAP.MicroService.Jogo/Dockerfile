FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["FIAP.MicroService.Jogo/FIAP.MicroService.Jogo.csproj", "FIAP.MicroService.Jogo/"]
RUN dotnet restore "FIAP.MicroService.Jogo/FIAP.MicroService.Jogo.csproj"
COPY . .
WORKDIR "/src/FIAP.MicroService.Jogo"
RUN dotnet build "./FIAP.MicroService.Jogo.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./FIAP.MicroService.Jogo.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "FIAP.MicroService.Jogo.dll"]

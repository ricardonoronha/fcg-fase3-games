# ============================================================================
# FIAP Cloud Games - Microsserviço de Jogos
# Dockerfile Otimizado para Kubernetes - Fase 4
# ============================================================================

# -----------------------------------------------------------------------------
# Stage 1: Build
# -----------------------------------------------------------------------------
FROM mcr.microsoft.com/dotnet/sdk:8.0-alpine AS build

WORKDIR /src

# Copiar apenas arquivos de projeto primeiro (melhor cache de layers)
COPY FIAP.MicroService.Jogos.API/*.csproj ./FIAP.MicroService.Jogos.API/
COPY FIAP.MicroService.Jogos.Dominio/*.csproj ./FIAP.MicroService.Jogos.Dominio/
COPY FIAP.MicroService.Jogos.Infraestrutura/*.csproj ./FIAP.MicroService.Jogos.Infraestrutura/

# Restore de dependências
RUN dotnet restore ./FIAP.MicroService.Jogos.API/FIAP.MicroService.Jogos.API.csproj

# Copiar código fonte
COPY FIAP.MicroService.Jogos.API/ ./FIAP.MicroService.Jogos.API/
COPY FIAP.MicroService.Jogos.Dominio/ ./FIAP.MicroService.Jogos.Dominio/
COPY FIAP.MicroService.Jogos.Infraestrutura/ ./FIAP.MicroService.Jogos.Infraestrutura/

# Build da aplicação
WORKDIR /src/FIAP.MicroService.Jogos.API
RUN dotnet build -c Release -o /app/build

# -----------------------------------------------------------------------------
# Stage 2: Publish
# -----------------------------------------------------------------------------
FROM build AS publish

RUN dotnet publish -c Release -o /app/publish /p:UseAppHost=false

# -----------------------------------------------------------------------------
# Stage 3: Runtime (Imagem Final)
# -----------------------------------------------------------------------------
FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine AS final

LABEL maintainer="Grupo 118 - FIAP Tech Challenge" \
      version="4.0" \
      description="FCG Jogos API - Microsserviço de Jogos"

# Instalar dependências necessárias para DataDog em Alpine
RUN apk add --no-cache icu-libs libc6-compat libgcc libstdc++

# Criar usuário non-root
RUN addgroup -g 1000 appgroup && \
    adduser -u 1000 -G appgroup -D -s /bin/sh appuser

WORKDIR /app

# Copiar binários publicados
COPY --from=publish /app/publish .

# Ajustar permissões
RUN chown -R appuser:appgroup .

USER appuser

EXPOSE 8080

ENV ASPNETCORE_URLS=http://+:8080 \
    ASPNETCORE_ENVIRONMENT=Production \
    DOTNET_RUNNING_IN_CONTAINER=true \
    DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false

ENTRYPOINT ["dotnet", "FIAP.MicroService.Jogos.API.dll"]

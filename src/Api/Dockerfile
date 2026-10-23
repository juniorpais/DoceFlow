# Build multi-stage da API DoceFlow (spec §22.1).
# Contexto de build: RAIZ do repositório.
#   docker build -f src/Api/Dockerfile .

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_HTTP_PORTS=8080
# Não rodar como root (usuário "app" já existe na imagem aspnet 8.0)
USER app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Configuração compartilhada e de analyzers (antes do restore)
COPY ["Directory.Build.props", "./"]
COPY [".editorconfig", "./"]

# Só os .csproj primeiro → cache de restore entre builds
COPY ["src/Api/Api.csproj", "src/Api/"]
COPY ["src/Application/Application.csproj", "src/Application/"]
COPY ["src/Domain/Domain.csproj", "src/Domain/"]
COPY ["src/Infrastructure/Infrastructure.csproj", "src/Infrastructure/"]
COPY ["src/Common/Common.csproj", "src/Common/"]
RUN dotnet restore "src/Api/Api.csproj"

COPY src/ src/
WORKDIR /src/src/Api
RUN dotnet publish "Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish --no-restore /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Api.dll"]

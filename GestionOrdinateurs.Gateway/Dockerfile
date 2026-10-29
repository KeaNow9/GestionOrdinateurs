# =========================
# Build
# =========================
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /app

# Copie du csproj puis restauration des packages (pas besoin de Shared)
COPY ./GestionOrdinateurs.Gateway/GestionOrdinateurs.Gateway.csproj ./GestionOrdinateurs.Gateway/
RUN dotnet restore ./GestionOrdinateurs.Gateway

# Copie du code source puis publication
COPY ./GestionOrdinateurs.Gateway/ ./GestionOrdinateurs.Gateway/
RUN dotnet publish ./GestionOrdinateurs.Gateway -c Release -o out

# =========================
# Runtime
# =========================
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app/out ./

# Environnement de développement
ENV ASPNETCORE_ENVIRONMENT=Development

# Port de la Gateway
EXPOSE 5010

# Lancement de la Gateway
ENTRYPOINT ["dotnet", "GestionOrdinateurs.Gateway.dll"]

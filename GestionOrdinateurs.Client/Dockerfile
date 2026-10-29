# =========================
# Build
# =========================
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /app

# Copie des csproj puis restauration des packages
COPY ./GestionOrdinateurs.Client/GestionOrdinateurs.Client.csproj ./GestionOrdinateurs.Client/
COPY ./GestionOrdinateurs.Shared/GestionOrdinateurs.Shared.csproj ./GestionOrdinateurs.Shared/
RUN dotnet restore ./GestionOrdinateurs.Client

# Copie du code source puis publication
COPY ./GestionOrdinateurs.Client/ ./GestionOrdinateurs.Client/
COPY ./GestionOrdinateurs.Shared/ ./GestionOrdinateurs.Shared/
RUN dotnet publish ./GestionOrdinateurs.Client -c Release -o out

# =========================
# Runtime (nginx sert les fichiers du front)
# =========================
FROM nginx:alpine AS runtime

# Copie des fichiers générés par Blazor
COPY --from=build /app/out/wwwroot /usr/share/nginx/html

# Configuration de nginx (port 5000)
COPY ./GestionOrdinateurs.Client/nginx.conf /etc/nginx/conf.d/default.conf

# Port du front
EXPOSE 5000

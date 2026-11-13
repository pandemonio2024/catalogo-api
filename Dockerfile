# Etapa 1: compilar y publicar con el SDK
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Se copian primero los .csproj para aprovechar la caché de restore
COPY CatalogoApi.sln ./
COPY src/Catalogo.Dominio/*.csproj src/Catalogo.Dominio/
COPY src/Catalogo.Aplicacion/*.csproj src/Catalogo.Aplicacion/
COPY src/Catalogo.Infraestructura/*.csproj src/Catalogo.Infraestructura/
COPY src/Catalogo.Api/*.csproj src/Catalogo.Api/
COPY tests/Catalogo.Pruebas/*.csproj tests/Catalogo.Pruebas/
RUN dotnet restore

COPY . .
RUN dotnet publish src/Catalogo.Api/Catalogo.Api.csproj -c Release -o /app --no-restore

# Etapa 2: imagen ligera solo con el runtime de ASP.NET
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
COPY --from=build /app .
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080
USER $APP_UID
ENTRYPOINT ["dotnet", "Catalogo.Api.dll"]

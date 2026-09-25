FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copiamos primero el csproj para que la capa de restore se cachee entre despliegues.
COPY EntrenamientoFuerza.csproj ./
RUN dotnet restore EntrenamientoFuerza.csproj

COPY . .
RUN dotnet publish EntrenamientoFuerza.csproj -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app

# Npgsql carga GSSAPI al negociar la autenticación con el servidor, y la imagen de aspnet
# no trae Kerberos: sin esto la conexión muere con "Cannot load library libgssapi_krb5.so.2".
# Va antes del COPY para que la capa se cachee entre despliegues.
RUN apt-get update \
    && apt-get install -y --no-install-recommends libgssapi-krb5-2 \
    && rm -rf /var/lib/apt/lists/*

COPY --from=build /app/publish ./

ENV ASPNETCORE_ENVIRONMENT=Production

# El límite de instancias inotify (fs.inotify.max_user_instances) es del host y Render lo
# reparte entre todos los contenedores. Los watchers de .NET lo agotan y CreateBuilder muere
# con "The configured user limit (128) on the number of inotify instances has been reached".
# Con estas dos variables la app no abre ni una instancia inotify:
#   - no recargar la configuración al cambiar los appsettings (en producción no cambian),
#   - y que cualquier watcher restante (wwwroot, assets estáticos) use sondeo en vez de inotify.
ENV DOTNET_hostBuilder__reloadConfigOnChange=false
ENV DOTNET_USE_POLLING_FILE_WATCHER=1

EXPOSE 8080

# Render inyecta $PORT. exec deja a dotnet como PID 1 para que reciba el SIGTERM del apagado.
ENTRYPOINT ["/bin/sh", "-c", "exec dotnet EntrenamientoFuerza.dll --urls=http://+:${PORT:-8080}"]

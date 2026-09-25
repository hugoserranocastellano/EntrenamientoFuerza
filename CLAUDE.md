# CLAUDE.md

Guía para trabajar en este repositorio.

## Qué es esto

App familiar de entrenamiento de fuerza: catálogo de ejercicios (nombre, descripción,
vídeo de YouTube), rutinas (series de ejercicios con repeticiones o duración y
descanso) y un diario donde cada usuario registra lo que realmente hizo cada día,
serie a serie. Blazor Server (.NET 10, `net10.0`) sobre PostgreSQL (Supabase),
pensada para desplegarse en Render — mismo patrón que `CiudadDeportivaTudela`.

## Comandos

```bash
dotnet build
dotnet ef migrations add <Nombre>   # tras tocar ApplicationDbContext u OnModelCreating
dotnet ef database update           # aplicar migraciones pendientes
```

Arrancar en local:

```bash
dotnet build
ASPNETCORE_ENVIRONMENT=Development ASPNETCORE_URLS=http://localhost:5200 \
  dotnet bin/Debug/net10.0/EntrenamientoFuerza.dll
```

No hay pantalla de registro pública. El primer usuario (y cualquier alta o reseteo de
contraseña) se crea con:

```bash
dotnet bin/Debug/net10.0/EntrenamientoFuerza.dll crear-usuario <usuario> "<nombre>" <password> [rol]
```

## Configuración y secretos

`ConnectionStrings:Supabase`, igual que en `CiudadDeportivaTudela`:
- Local: `dotnet user-secrets set "ConnectionStrings:Supabase" "..."`.
- Render: variable de entorno `ConnectionStrings__Supabase`.

Debe ser la cadena del **pooler** de Supabase (`*.pooler.supabase.com:5432`, session
mode), no la conexión directa (solo IPv6). Usuario con sufijo `postgres.<project-ref>`.
`Data/PostgresConnectionString.cs` valida esto al arrancar.

## Arquitectura

- **Blazor Server**, páginas en `Components/Pages/*.razor`, render mode interactivo
  salvo `Login.razor` (necesita el `HttpContext` real para `SignInAsync`).
- **Acceso a datos**: `IDbContextFactory<ApplicationDbContext>` inyectado, nunca el
  `DbContext` directo (ver razón en `CiudadDeportivaTudela/CLAUDE.md`).
- **Mapeo EF Core** centralizado en `Data/ApplicationDbContext.OnModelCreating`
  (snake_case). Modelos en `Models/*.cs` son POCOs sin atributos.
- **Autenticación propia**: usuario + contraseña hasheada con
  `Microsoft.AspNetCore.Identity.PasswordHasher` (`Services/UsuarioAuth.cs`), cookie
  auth con el scheme por defecto. No usa el número de socio/teléfono del otro proyecto.
- **Modelo de datos**: `Ejercicio` → `Rutina`/`RutinaEjercicio` (plan) →
  `SesionDiario`/`SesionEjercicio`/`SesionSerie` (lo realmente hecho, independiente
  del plan). Repeticiones y duración son mutuamente excluyentes en cada fila.
- **Vídeos**: solo enlace embebido de YouTube (`Ejercicio.VideoUrl`), sin subida ni
  descarga de archivos.
- **Despliegue en Render**: mismo patrón que `CiudadDeportivaTudela` (ver ese
  `CLAUDE.md` para el porqué de cada detalle — `ForwardedHeaders`, `/healthz`,
  `numInstances: 1`, no `UseHttpsRedirection()` en producción).

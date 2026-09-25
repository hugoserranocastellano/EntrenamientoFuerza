using EntrenamientoFuerza.Components;
using EntrenamientoFuerza.Data;
using EntrenamientoFuerza.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.Cookie.Name = "EntrenamientoFuerza.Usuario";
        options.LoginPath = "/";
        options.LogoutPath = "/logout";
        options.AccessDeniedPath = "/";
        options.ExpireTimeSpan = TimeSpan.FromDays(30);
        options.SlidingExpiration = true;
    });

builder.Services.AddAuthorization();
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<UsuarioAuth>();

// Render termina el TLS en su proxy y reenvía la petición por HTTP. Sin esto,
// Request.Scheme sería "http" y las URLs absolutas saldrían mal.
builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
    // La IP del proxy de Render no se conoce de antemano.
    options.KnownIPNetworks.Clear();
    options.KnownProxies.Clear();
});

var rawConnectionString = builder.Configuration.GetConnectionString("Supabase");
if (string.IsNullOrWhiteSpace(rawConnectionString))
{
    throw new InvalidOperationException("Falta la cadena de conexión 'Supabase'.");
}

// Acepta tanto el formato clave=valor como la URI postgresql:// que copia Supabase.
var connectionString = PostgresConnectionString.Normalize(rawConnectionString);

builder.Services.AddDbContextFactory<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString));

var app = builder.Build();

// No hay pantalla de registro a propósito (los usuarios los da de alta el admin).
// `dotnet EntrenamientoFuerza.dll crear-usuario <usuario> "<nombre>" <password> [rol]`
// crea o resetea la contraseña de un usuario y termina sin levantar el servidor web.
if (args.Length > 0 && args[0] == "crear-usuario")
{
    await CrearUsuarioAsync(app, args);
    return;
}

// Sin esto, un 28P01 no permite distinguir entre contraseña mala y usuario mal formado.
// El resumen no incluye la contraseña, sólo su longitud.
app.Logger.LogInformation("Conexión a Supabase: {Resumen}", PostgresConnectionString.Describe(connectionString));

app.UseForwardedHeaders();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
    // Nada de UseHttpsRedirection aquí: Render ya redirige HTTP -> HTTPS en el borde,
    // y hacerlo otra vez dentro rompería el health check interno, que llega por HTTP.
}
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);

app.UseAuthentication();
app.UseAuthorization();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

// Cerrar sesión. Es POST a propósito: un enlace GET lo dispararía cualquier
// prefetch del navegador y cerraría la sesión sin que el usuario lo pida.
app.MapPost("/logout", async (HttpContext context) =>
{
    await context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
    return Results.LocalRedirect("/");
});

// Health check de Render. A propósito no toca la base de datos: si Supabase
// tarda en responder no queremos que Render reinicie el contenedor.
app.MapGet("/healthz", () => Results.Text("ok"));

app.Run();

static async Task CrearUsuarioAsync(WebApplication app, string[] args)
{
    if (args.Length < 4)
    {
        Console.WriteLine("Uso: crear-usuario <nombre-usuario> \"<nombre completo>\" <password> [rol]");
        return;
    }

    var nombreUsuario = args[1];
    var nombre = args[2];
    var password = args[3];
    var rol = args.Length > 4 ? args[4] : "Usuario";

    using var scope = app.Services.CreateScope();
    var dbFactory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<ApplicationDbContext>>();
    await using var db = await dbFactory.CreateDbContextAsync();

    var hasher = new Microsoft.AspNetCore.Identity.PasswordHasher<EntrenamientoFuerza.Models.Usuario>();
    var usuario = await db.Usuarios.FirstOrDefaultAsync(u => u.NombreUsuario == nombreUsuario);

    if (usuario is null)
    {
        usuario = new EntrenamientoFuerza.Models.Usuario
        {
            NombreUsuario = nombreUsuario,
            Nombre = nombre,
            Rol = rol,
            FechaAlta = DateTime.UtcNow,
        };
        usuario.PasswordHash = hasher.HashPassword(usuario, password);
        db.Usuarios.Add(usuario);
        Console.WriteLine($"Creando usuario '{nombreUsuario}'...");
    }
    else
    {
        usuario.Nombre = nombre;
        usuario.Rol = rol;
        usuario.PasswordHash = hasher.HashPassword(usuario, password);
        usuario.Activo = true;
        Console.WriteLine($"Actualizando usuario '{nombreUsuario}'...");
    }

    await db.SaveChangesAsync();
    Console.WriteLine("Hecho.");
}

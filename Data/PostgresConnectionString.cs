using Npgsql;

namespace EntrenamientoFuerza.Data;

/// <summary>
/// Supabase muestra la cadena de conexión en formato URI (postgresql://usuario:clave@host:puerto/base),
/// pero Npgsql sólo entiende el formato clave=valor. Si se pega la URI tal cual, ADO.NET revienta con
/// "Format of the initialization string does not conform to specification starting at index N".
/// </summary>
public static class PostgresConnectionString
{
    public static string Normalize(string value)
    {
        // Al copiar el valor a la consola de Render es fácil arrastrar espacios, un salto de
        // línea o las comillas con las que venía envuelto en un fichero de ejemplo.
        var raw = value.Trim().Trim('"', '\'').Trim();

        NpgsqlConnectionStringBuilder builder;

        if (raw.StartsWith("postgres://", StringComparison.OrdinalIgnoreCase)
            || raw.StartsWith("postgresql://", StringComparison.OrdinalIgnoreCase))
        {
            builder = FromUri(raw);
        }
        else
        {
            // Validamos aquí para fallar con un mensaje que explique el formato esperado
            // en vez del "initialization string" opaco de ADO.NET.
            try
            {
                builder = new NpgsqlConnectionStringBuilder(raw);
            }
            catch (ArgumentException ex)
            {
                // Incluimos longitud y las claves que sí se reconocieron: sitúan el fallo sin
                // volcar la contraseña en los logs de Render.
                throw new InvalidOperationException(
                    "La cadena de conexión 'Supabase' no tiene un formato válido. Usa clave=valor "
                    + "(Host=...;Port=5432;Database=postgres;Username=...;Password=...;SSL Mode=Require) "
                    + "o la URI postgresql://usuario:clave@host:puerto/base. Si la contraseña contiene "
                    + "';' o '=', entrecomíllala con \" o usa la URI con la clave URL-encoded. "
                    + $"Detalle: {ex.Message} (longitud del valor: {raw.Length}; "
                    + $"claves antes del fallo: {DescribeKeys(raw)}).", ex);
            }
        }

        Validate(builder);

        return builder.ConnectionString;
    }

    /// <summary>
    /// Resumen sin secretos para los logs y la página de error: permite verificar qué se está
    /// enviando realmente a Supabase sin exponer la contraseña.
    /// </summary>
    public static string Describe(string normalizedConnectionString)
    {
        var builder = new NpgsqlConnectionStringBuilder(normalizedConnectionString);

        return $"Host={builder.Host}; Port={builder.Port}; Database={builder.Database}; "
            + $"Username={builder.Username}; SslMode={builder.SslMode}; "
            + $"Password=({builder.Password?.Length ?? 0} caracteres)";
    }

    /// <summary>
    /// Errores que Postgres sólo sabe reportar como 28P01 ("password authentication failed"),
    /// aunque la contraseña sea correcta. Mejor detectarlos al arrancar que a base de despliegues.
    /// </summary>
    private static void Validate(NpgsqlConnectionStringBuilder builder)
    {
        var host = builder.Host ?? string.Empty;
        var user = builder.Username ?? string.Empty;
        var password = builder.Password ?? string.Empty;

        // El pooler de Supabase multiplexa varios proyectos en el mismo host y decide a cuál
        // te conecta por el sufijo del usuario. Con "postgres" a secas no puede resolver el
        // tenant y devuelve 28P01 aunque la contraseña sea la buena.
        if (host.Contains("pooler.supabase.com", StringComparison.OrdinalIgnoreCase)
            && !user.Contains('.'))
        {
            throw new InvalidOperationException(
                $"El usuario '{user}' no sirve contra el pooler de Supabase ({host}): al ir por "
                + "pooler el usuario debe ser 'postgres.<project-ref>' (por ejemplo "
                + "'postgres.abcdefghijklmnopqrst'). El project-ref es el subdominio de la URL de "
                + "tu proyecto en Supabase. Cópialo de Supabase > Connect > Connection pooling, "
                + "que ya trae el usuario correcto. Con 'postgres' a secas Postgres responde "
                + "28P01 (password authentication failed) por muchas veces que cambies la clave.");
        }

        if (password.Length == 0)
        {
            throw new InvalidOperationException(
                "La cadena de conexión 'Supabase' no lleva contraseña. Si la pegaste como URI, "
                + "revisa que no se haya quedado el placeholder ni un '@' sin codificar.");
        }

        // Supabase entrega la URI con [YOUR-PASSWORD] a rellenar; es fácil desplegar sin sustituirlo,
        // o sustituirlo dejando los corchetes puestos.
        if (password.Contains("YOUR-PASSWORD", StringComparison.OrdinalIgnoreCase)
            || password.Contains("YOUR_PASSWORD", StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException(
                "La contraseña sigue siendo el placeholder [YOUR-PASSWORD] de Supabase. "
                + "Sustitúyelo por la contraseña real de la base de datos.");
        }

        if (password.StartsWith('[') && password.EndsWith(']'))
        {
            throw new InvalidOperationException(
                "La contraseña va entre corchetes. Los corchetes de [YOUR-PASSWORD] eran sólo el "
                + "hueco a rellenar: quítalos y deja la contraseña sola.");
        }
    }

    /// <summary>
    /// Lista los nombres de clave (nunca los valores) de los segmentos separados por ';'.
    /// </summary>
    private static string DescribeKeys(string raw)
    {
        var keys = raw.Split(';')
            .Where(segment => segment.Trim().Length > 0)
            .Select(segment =>
            {
                var parts = segment.Split('=', 2);
                var key = parts[0].Trim();

                // Un segmento sin '=' (o con un nombre que no parece una clave) suele ser un
                // trozo de la contraseña partido por un ';'. No lo reproducimos.
                return parts.Length == 2 && key.Length <= 30 && key.All(c => char.IsLetter(c) || c is ' ' or '_')
                    ? key
                    : "<segmento inválido>";
            });

        return string.Join(", ", keys);
    }

    /// <summary>
    /// Parte la URI a mano en vez de con <see cref="Uri"/>: las contraseñas de Supabase llevan
    /// símbolos ('/', '?', '#') que Uri interpretaría como separadores de ruta o query y que
    /// mucha gente pega sin URL-encodear.
    /// </summary>
    private static NpgsqlConnectionStringBuilder FromUri(string raw)
    {
        var rest = raw[(raw.IndexOf("://", StringComparison.Ordinal) + 3)..];

        // La query (?sslmode=..., ?pgbouncer=true) la ignoramos: el SslMode lo fijamos nosotros
        // y los flags de pgbouncer no aplican a Npgsql.
        var queryStart = rest.IndexOf('?');
        if (queryStart >= 0)
        {
            rest = rest[..queryStart];
        }

        // Último '@': cualquier '@' anterior forma parte de la contraseña.
        var atIndex = rest.LastIndexOf('@');
        if (atIndex < 0)
        {
            throw new InvalidOperationException(
                "La URI de conexión no tiene la forma postgresql://usuario:clave@host:puerto/base "
                + "(falta el '@' que separa las credenciales del host).");
        }

        var userInfo = rest[..atIndex];
        var hostPart = rest[(atIndex + 1)..];

        // Primer ':': el usuario nunca lleva ':', la contraseña sí puede.
        var colonIndex = userInfo.IndexOf(':');
        var user = colonIndex < 0 ? userInfo : userInfo[..colonIndex];
        var password = colonIndex < 0 ? string.Empty : userInfo[(colonIndex + 1)..];

        var slashIndex = hostPart.IndexOf('/');
        var database = slashIndex < 0 ? string.Empty : hostPart[(slashIndex + 1)..];
        var hostAndPort = slashIndex < 0 ? hostPart : hostPart[..slashIndex];

        var portIndex = hostAndPort.LastIndexOf(':');
        var host = portIndex < 0 ? hostAndPort : hostAndPort[..portIndex];
        var port = 5432;
        if (portIndex >= 0 && !int.TryParse(hostAndPort[(portIndex + 1)..], out port))
        {
            throw new InvalidOperationException(
                $"El puerto '{hostAndPort[(portIndex + 1)..]}' de la URI de conexión no es un número.");
        }

        return new NpgsqlConnectionStringBuilder
        {
            Host = host,
            Port = port,
            Database = database.Length == 0 ? "postgres" : database,
            Username = Unescape(user),
            Password = Unescape(password),
            // Supabase exige TLS y su certificado no está en el almacén del contenedor.
            SslMode = SslMode.Require,
        };
    }

    /// <summary>
    /// Sólo deshace el %XX si el texto está realmente percent-encoded. Una contraseña con un '%'
    /// literal (pegada sin codificar) se quedaría mutilada por un UnescapeDataString a ciegas.
    /// </summary>
    private static string Unescape(string value)
    {
        var decoded = Uri.UnescapeDataString(value);
        return decoded.Length == value.Length ? value : decoded;
    }
}

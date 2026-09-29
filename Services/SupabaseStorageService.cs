namespace EntrenamientoFuerza.Services;

/// <summary>
/// Sube y borra ficheros en Supabase Storage vía su API REST, usando la service_role key
/// (server-side, salta RLS). Los buckets son públicos, así que la URL devuelta es fija y
/// directamente lo que se guarda en las columnas url_foto*.
/// </summary>
public class SupabaseStorageService
{
    private readonly HttpClient _http;
    private readonly string _baseUrl;

    public SupabaseStorageService(HttpClient http, IConfiguration configuration)
    {
        _http = http;

        var url = configuration["Supabase:Url"];
        var serviceRoleKey = configuration["Supabase:ServiceRoleKey"];

        if (string.IsNullOrWhiteSpace(url) || string.IsNullOrWhiteSpace(serviceRoleKey))
        {
            throw new InvalidOperationException(
                "Faltan 'Supabase:Url' o 'Supabase:ServiceRoleKey' en la configuración.");
        }

        _baseUrl = url.TrimEnd('/');
        _http.DefaultRequestHeaders.Authorization = new("Bearer", serviceRoleKey);
        _http.DefaultRequestHeaders.Add("apikey", serviceRoleKey);
    }

    /// <summary>
    /// Sube (o sobrescribe) <paramref name="bucket"/>/<paramref name="path"/> y devuelve la URL
    /// pública resultante. <paramref name="path"/> no lleva barra inicial.
    /// </summary>
    public async Task<string> SubirAsync(
        string bucket, string path, Stream contenido, string contentType, CancellationToken ct = default)
    {
        using var streamContent = new StreamContent(contenido);
        streamContent.Headers.ContentType = new(contentType);

        var request = new HttpRequestMessage(HttpMethod.Put, $"{_baseUrl}/storage/v1/object/{bucket}/{path}")
        {
            Content = streamContent,
        };
        // Permite subir encima de un fichero que ya existe (mismo id.jpg al reeditar).
        request.Headers.Add("x-upsert", "true");

        using var response = await _http.SendAsync(request, ct);
        if (!response.IsSuccessStatusCode)
        {
            var detalle = await response.Content.ReadAsStringAsync(ct);
            throw new InvalidOperationException(
                $"Error subiendo a Supabase Storage ({(int)response.StatusCode}): {detalle}");
        }

        return $"{_baseUrl}/storage/v1/object/public/{bucket}/{path}";
    }

    public async Task BorrarAsync(string bucket, string path, CancellationToken ct = default)
    {
        using var response = await _http.DeleteAsync($"{_baseUrl}/storage/v1/object/{bucket}/{path}", ct);
        // Si ya no existe (404), no pasa nada: el resultado que queríamos ya se cumple.
        if (!response.IsSuccessStatusCode && response.StatusCode != System.Net.HttpStatusCode.NotFound)
        {
            var detalle = await response.Content.ReadAsStringAsync(ct);
            throw new InvalidOperationException(
                $"Error borrando de Supabase Storage ({(int)response.StatusCode}): {detalle}");
        }
    }
}

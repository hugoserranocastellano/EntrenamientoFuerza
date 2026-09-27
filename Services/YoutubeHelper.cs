using Microsoft.AspNetCore.WebUtilities;

namespace EntrenamientoFuerza.Services;

public static class YoutubeHelper
{
    public static string? ExtraerId(string url)
    {
        if (!Uri.TryCreate(url, UriKind.Absolute, out var uri))
            return null;

        if (uri.Host.Contains("youtu.be", StringComparison.OrdinalIgnoreCase))
            return uri.AbsolutePath.Trim('/');

        if (uri.Host.Contains("youtube.com", StringComparison.OrdinalIgnoreCase))
        {
            if (uri.AbsolutePath.StartsWith("/shorts/", StringComparison.OrdinalIgnoreCase))
                return uri.AbsolutePath["/shorts/".Length..].Trim('/');

            var query = QueryHelpers.ParseQuery(uri.Query);
            return query.TryGetValue("v", out var v) ? v.ToString() : null;
        }

        return null;
    }

    public static string EmbedUrl(string url)
    {
        var id = ExtraerId(url);
        return id is null ? url : $"https://www.youtube.com/embed/{id}";
    }

    public static string? MiniaturaUrl(string? url)
    {
        if (string.IsNullOrWhiteSpace(url)) return null;
        var id = ExtraerId(url);
        return id is null ? null : $"https://img.youtube.com/vi/{id}/hqdefault.jpg";
    }

    public static string UrlBusqueda(string terminos) =>
        $"https://www.youtube.com/results?search_query={Uri.EscapeDataString(terminos)}";
}

public static class ImagenHelper
{
    // La imagen propia (subida a mano, más estandarizada) tiene prioridad; si no hay,
    // se recurre a la miniatura del vídeo de YouTube como alternativa.
    public static string? MiniaturaDe(Models.Ejercicio ejercicio) =>
        !string.IsNullOrWhiteSpace(ejercicio.ImagenUrl) ? ejercicio.ImagenUrl : YoutubeHelper.MiniaturaUrl(ejercicio.VideoUrl);

    public static string UrlBusquedaImagenes(string terminos) =>
        $"https://www.google.com/search?tbm=isch&q={Uri.EscapeDataString(terminos)}";
}

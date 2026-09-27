// Lee el portapapeles para el botón "Pegar" del enlace de YouTube. Requiere HTTPS
// (o localhost) y gesto del usuario (click), que es justo cómo se llama desde Blazor.
export async function leer() {
    try {
        return await navigator.clipboard.readText();
    } catch {
        return null;
    }
}

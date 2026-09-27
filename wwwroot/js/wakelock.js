// Mantiene la pantalla encendida durante una rutina activa. La Screen Wake Lock API
// se libera sola si el usuario cambia de pestaña/app, así que la volvemos a pedir
// al recuperar el foco mientras siga marcada como activa desde Blazor.
let wakeLock = null;
let activa = false;

async function pedir() {
    if (!('wakeLock' in navigator)) return;
    try {
        wakeLock = await navigator.wakeLock.request('screen');
    } catch {
        // Puede fallar si la pestaña no está visible; se reintentará en visibilitychange.
    }
}

export async function activar() {
    activa = true;
    await pedir();
}

export async function desactivar() {
    activa = false;
    if (wakeLock) {
        try { await wakeLock.release(); } catch { /* ya liberado */ }
        wakeLock = null;
    }
}

document.addEventListener('visibilitychange', () => {
    if (activa && document.visibilityState === 'visible' && !wakeLock) {
        pedir();
    }
});

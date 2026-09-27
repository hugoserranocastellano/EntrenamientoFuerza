// Service worker mínimo: sólo existe para que el navegador considere la app
// instalable (PWA). Esta app es Blazor Server y necesita una conexión SignalR
// activa, así que no tiene sentido cachear páginas para uso offline: si no hay
// conexión, la app no puede funcionar igualmente.
self.addEventListener('install', () => {
    self.skipWaiting();
});

self.addEventListener('activate', (event) => {
    event.waitUntil(self.clients.claim());
});

self.addEventListener('fetch', () => {
    // Passthrough: dejamos que cada petición vaya a la red normalmente.
});

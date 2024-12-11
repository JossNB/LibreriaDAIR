const CACHE_NAME = "pwa-cache-v1";
const urlsToCache = [
    "/",
    "/css/OnboardingStyle.css",
    "/Content/icons/icon-192x192.png",
    "/Content/icons/icon-512x512.png",
    "/Imagenes/Categorias.png",
    "/Imagenes/DetallesLibro.png",
    "/Imagenes/Pasarela.png",
    "/Imagenes/Verificacion.png",
    "/Imagenes/DigitalLibrary.jpg",
    "/Imagenes/LogoDAIR.png",
    "/Imagenes/EncargoLibro.png",
    "/Imagenes/EnviosIcono.png",
    "/Imagenes/LibrosFantasia.png",
    "/Imagenes/LibrosInfantiles.png",
    "/Imagenes/LibrosNovelaGrafica.jpg",
    "/Imagenes/LibrosRomance.png",
    "/Imagenes/LogoOfertas.png",
    "/Imagenes/LogoServicios.png",
    "/Imagenes/ProveedoresIcono.png",
    "/Imagenes/ReseniasIcono.png"

];

self.addEventListener("install", (event) => {
    event.waitUntil(
        caches.open(CACHE_NAME).then((cache) => {
            console.log("Archivos en caché");
            return cache.addAll(urlsToCache);
        })
    );
});

self.addEventListener("fetch", (event) => {
    event.respondWith(
        caches.match(event.request).then((response) => {
            return response || fetch(event.request);
        })
    );
});

self.addEventListener("activate", (event) => {
    const cacheWhitelist = [CACHE_NAME];
    event.waitUntil(
        caches.keys().then((cacheNames) => {
            return Promise.all(
                cacheNames.map((cacheName) => {
                    if (!cacheWhitelist.includes(cacheName)) {
                        return caches.delete(cacheName)
                    }
                })
            )
        })
    )
})
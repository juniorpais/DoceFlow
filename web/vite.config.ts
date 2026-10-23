import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'
import { VitePWA } from 'vite-plugin-pwa'

export default defineConfig({
  plugins: [
    react(),
    VitePWA({
      registerType: 'autoUpdate',
      includeAssets: ['icon.svg', 'apple-touch-icon.png'],
      manifest: {
        name: 'DoceFlow',
        short_name: 'DoceFlow',
        description: 'Doces prontos e encomendas sob medida.',
        lang: 'pt-BR',
        start_url: '/app',
        scope: '/',
        display: 'standalone',
        theme_color: '#d9486b',
        background_color: '#fff7f2',
        icons: [
          { src: 'pwa-192x192.png', sizes: '192x192', type: 'image/png', purpose: 'any' },
          { src: 'pwa-512x512.png', sizes: '512x512', type: 'image/png', purpose: 'any' },
          { src: 'icon.svg', sizes: 'any', type: 'image/svg+xml', purpose: 'any' },
        ],
        shortcuts: [
          { name: 'Vitrine', url: '/app' },
          { name: 'Minhas encomendas', url: '/app/encomendas' },
          { name: 'PDV', url: '/pdv' },
          { name: 'Cozinha', url: '/kds' },
        ],
      },
      workbox: {
        navigateFallback: '/index.html',
        navigateFallbackDenylist: [/^\/api\//],
        runtimeCaching: [
          {
            urlPattern: ({ url }) => url.pathname.startsWith('/api/capacidade') || url.pathname.startsWith('/api/v1/capacidade'),
            handler: 'NetworkOnly',
          },
          {
            urlPattern: ({ url }) => url.pathname.startsWith('/api/catalogo') || url.pathname.startsWith('/api/v1/catalogo'),
            handler: 'StaleWhileRevalidate',
            options: { cacheName: 'api-catalogo' },
          },
          {
            urlPattern: ({ request }) => request.destination === 'image',
            handler: 'CacheFirst',
            options: {
              cacheName: 'imagens',
              expiration: { maxEntries: 200, maxAgeSeconds: 60 * 60 * 24 * 30 },
              cacheableResponse: { statuses: [0, 200] },
            },
          },
        ],
      },
      devOptions: { enabled: false },
    }),
  ],
  server: {
    proxy: {
      '/api': { target: 'http://localhost:8080', changeOrigin: true },
    },
  },
})

# DoceFlow Web (PWA)

React 18 + TypeScript + Vite + Tailwind, com `vite-plugin-pwa` (Workbox).

## Scripts

- `npm run dev`: servidor de desenvolvimento (proxy `/api` para `http://localhost:8080`)
- `npm run build`: `tsc -b && vite build` (gera `dist/` com service worker e manifest)
- `npm run preview`: serve o build local
- `npm run lint`: ESLint
- `npm run typecheck`: checagem de tipos

Copie `.env.example` para `.env` se precisar mudar `VITE_API_URL`.

Ícones do PWA em `public/` são placeholders (PNG gerados + `icon.svg`); substituir pela arte final.

Docker: `docker build -t doceflow-web .` (nginx; `PORT` configurável; `/api/` vai para o serviço `api:8080`).

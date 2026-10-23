# ADR 006 — Front-end em React + TypeScript como PWA único

## Status
Accepted

## Context
A aplicação precisa ser responsiva (web), instalável no celular e no desktop, e funcionar offline no PDV, KDS e entregador. Alternativas: Blazor WebAssembly (mesma linguagem do back), React/Vite, Flutter Web.

## Decision
React 18 + TypeScript + Vite + `vite-plugin-pwa` (Workbox), TanStack Query, Dexie para outbox offline, SignalR para tempo real. Um único app com layouts por formato (`/app`, `/pdv`, `/kds`, `/entrega`, `/admin`).

## Consequences
**Positivo:** melhor suporte a PWA/offline/Web APIs (câmera, Web Serial, push) e ecossistema de UI; bundle inicial menor que Blazor WASM; contratação mais fácil.
**Negativo:** duas linguagens no projeto; regras de validação espelhadas (Zod ↔ FluentValidation) exigem disciplina. Blazor WASM fica registrado como alternativa rejeitada por tamanho de bundle e maturidade de PWA/offline.

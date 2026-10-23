# ADR 003 — CQRS com Dispatcher customizado (sem MediatR)

## Status
Accepted

## Context
A especificação base já define um `Dispatcher` sobre `IServiceProvider` com registro manual de handlers.

## Decision
Adotar integralmente (§3 e §7). Queries de leitura (vitrine, fila do KDS, relatórios) usam repositórios de leitura com SQL otimizado e DTOs; commands usam agregados.

## Consequences
**Positivo:** sem dependência de MediatR, pipeline explícito, fácil de depurar.
**Negativo:** registro manual cresce com o número de features; mitigado com um teste que verifica se todo handler está registrado.

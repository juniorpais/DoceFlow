# ADR 005 — Outbox transacional para eventos com efeito financeiro

## Status
Accepted

## Context
A publicação best-effort com Polly (§15) aceita perder evento após 3 tentativas. Perder `EncomendaConfirmadaEvent` significa não reservar capacidade; perder `EncomendaCanceladaEvent` significa não estornar. Isso é inaceitável.

## Decision
Eventos marcados como `ICriticalDomainEvent` (`EncomendaConfirmada`, `EncomendaCancelada`, `PedidoPago`, `PagamentoConfirmado`) são gravados em `tblOutbox` **na mesma transação** da entidade. Um `OutboxWorker` publica e marca como enviado. Demais eventos (notificações, analytics) continuam best-effort com retry.

## Consequences
**Positivo:** at-least-once garantido para o que mexe com dinheiro e capacidade; consumidores idempotentes por `EventId`.
**Negativo:** tabela extra, worker extra, latência de até alguns segundos.

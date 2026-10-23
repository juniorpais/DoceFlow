# ADR 008 — PDV e KDS offline-first com outbox local e idempotência no servidor

## Status
Accepted

## Context
Internet instável no balcão não pode parar a venda. Operações precisam ser enfileiradas e sincronizadas sem duplicar.

## Decision
Front-end mantém outbox em IndexedDB (Dexie). Cada operação carrega `Idempotency-Key` (UUID gerado no cliente). `IdempotencyMiddleware` persiste chave → resposta por 24h em `tblIdempotencia` e devolve a mesma resposta em reenvios. Conflitos de estoque resultam em `409` exibido ao operador, nunca silenciados. Pix só é gerado online; offline aceita dinheiro/cartão (maquininha).

## Consequences
**Positivo:** venda nunca para; sincronização segura.
**Negativo:** estoque pode ficar temporariamente inconsistente entre canais; vitrine web sinaliza quando o PDV está offline há mais de 10 minutos.

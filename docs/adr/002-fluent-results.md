# ADR 002 — FluentResults em vez de exceções para fluxo de negócio

## Status
Accepted

## Context
"Sem capacidade", "orçamento expirado" e "pagamento insuficiente" são resultados esperados, não falhas.

## Decision
Handlers e entidades retornam `Result`/`Result<T>` com `Errors.Validation | NotFound | Conflict | CapacidadeIndisponivel | Pagamento`. `ToIResult()` mapeia para HTTP (422/404/409/402). Exceções ficam para bugs e infraestrutura (middleware §17).

## Consequences
**Positivo:** contratos HTTP previsíveis (RFC 7807); `409` de capacidade carrega `sugestoes[]`.
**Negativo:** verbosidade nos handlers; ausência de short-circuit automático.

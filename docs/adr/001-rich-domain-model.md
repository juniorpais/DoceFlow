# ADR 001 — Rich Domain Model com `Result`

## Status
Accepted

## Context
Regras como antecedência mínima, sinal de 50 %, FEFO e política de estorno são o coração do negócio e precisam ser testáveis sem infraestrutura.

## Decision
Regras vivem nas entidades (`Encomenda`, `Pedido`, `Lote`, `CapacidadeDia`) em métodos que retornam `Result` (FluentResults). Handlers apenas orquestram: carregar → chamar método → persistir → publicar eventos. Segue §14 da especificação técnica.

## Consequences
**Positivo:** regras em um só lugar; testes de entidade sem mocks; eventos verificáveis.
**Negativo:** entidades maiores; exige disciplina para não vazar regra para handlers.

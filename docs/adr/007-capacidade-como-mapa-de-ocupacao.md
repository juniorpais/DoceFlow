# ADR 007 — Capacidade de produção modelada como mapa de ocupação com strategies de impacto

## Status
Accepted

## Context
A especificação base modela disponibilidade de veículos via mapa de ocupação e `IImpactStrategy` por ação (retirar, devolver, cancelar). A cozinha tem o mesmo problema: recursos finitos por dia/turno/estação, ações que somam ou liberam carga.

## Decision
`CapacidadeDia (Dia, Turno, Estacao)` com `CapacidadeTotal`, `Ocupada`, `ReservaProntaEntrega`, `OverbookingPermitido`. Cada ação sobre encomenda (confirmar, cancelar, reagendar, alterar) tem uma `IImpactoCapacidadeStrategy` que produz deltas; um resolver aplica os deltas atomicamente (`UPDATE ... WHERE Ocupada + @delta <= limite`). Carga em "pontos" definidos por produto e opção.

## Consequences
**Positivo:** reaproveita padrão já validado; calendário do cliente consulta apenas `Disponivel()`; overbooking controlado e auditável.
**Negativo:** calibrar pontos de carga exige iteração com a cozinha (relatório estimado vs real).

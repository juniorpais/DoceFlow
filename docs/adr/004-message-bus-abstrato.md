# ADR 004 — Broker configurável: Log (dev) → RabbitMQ ou Azure Service Bus (prod)

## Status
Accepted

## Context
A especificação usa Azure Service Bus. Uma doceria de porte pequeno/médio pode não ter Azure, e o desenvolvimento local não deve depender de broker. Os consumidores assíncronos são: notificações, impacto na capacidade, reserva de insumos e jobs.

## Decision
Manter `IMessageBus`/`IEventPublisher` (§6, §15). Implementações: `LogEventPublisher` (dev/CI), `RabbitMqEventPublisher` (self-host, Docker Compose) e `ServiceBusEventPublisher` (Azure). Seleção por configuração `Messaging:Provider`. Sessões/ordenação por `EncomendaId`.

## Consequences
**Positivo:** roda localmente sem infraestrutura; escolha de nuvem adiada.
**Negativo:** duas implementações a manter; features exclusivas do Service Bus (sessions nativas) precisam de equivalente no RabbitMQ (fila por chave de roteamento).

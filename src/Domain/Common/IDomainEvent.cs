namespace Domain.Common;

public interface IDomainEvent
{
    Guid EventId { get; }

    DateTime OccurredAt { get; }
}

/// <summary>Evento crítico: deve ser persistido no Outbox (ADR-005).</summary>
public interface ICriticalDomainEvent : IDomainEvent
{
}

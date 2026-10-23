using Domain.Common;
using FluentAssertions;

namespace Unit.Domain;

public sealed class BaseEntityTests
{
    private sealed record EventoTeste : DomainEvent;

    private sealed class EntidadeTeste : BaseEntity
    {
        public EntidadeTeste() => Id = Guid.NewGuid();

        public void Fazer(IDomainEvent domainEvent) => AddDomainEvent(domainEvent);
    }

    [Fact(DisplayName = "Given an entity When adding events Then DomainEvents exposes them")]
    public void Given_Entity_When_AddingEvents_Then_DomainEventsExposesThem()
    {
        var entity = new EntidadeTeste();
        var first = new EventoTeste();
        var second = new EventoTeste();

        entity.Fazer(first);
        entity.Fazer(second);

        entity.DomainEvents.Should().HaveCount(2);
        entity.DomainEvents.Should().ContainInOrder(first, second);
    }

    [Fact(DisplayName = "Given an entity with events When clearing Then DomainEvents is empty")]
    public void Given_EntityWithEvents_When_Clearing_Then_DomainEventsIsEmpty()
    {
        var entity = new EntidadeTeste();
        entity.Fazer(new EventoTeste());

        entity.ClearDomainEvents();

        entity.DomainEvents.Should().BeEmpty();
    }

    [Fact(DisplayName = "Given two domain events When created Then EventIds are unique")]
    public void Given_TwoDomainEvents_When_Created_Then_EventIdsAreUnique()
    {
        var first = new EventoTeste();
        var second = new EventoTeste();

        first.EventId.Should().NotBeEmpty();
        first.EventId.Should().NotBe(second.EventId);
    }

    [Fact(DisplayName = "Given a domain event When created Then OccurredAt is UTC and recent")]
    public void Given_DomainEvent_When_Created_Then_OccurredAtIsUtcAndRecent()
    {
        var before = DateTime.UtcNow;

        var evento = new EventoTeste();

        evento.OccurredAt.Kind.Should().Be(DateTimeKind.Utc);
        evento.OccurredAt.Should().BeOnOrAfter(before).And.BeOnOrBefore(DateTime.UtcNow);
    }
}

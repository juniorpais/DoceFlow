using Application.Abstractions;

namespace Infrastructure.Events;

/// <summary>Relógio do sistema (UTC).</summary>
public sealed class SystemDateTimeProvider : IDateTimeProvider
{
    public DateTime UtcNow => DateTime.UtcNow;
}

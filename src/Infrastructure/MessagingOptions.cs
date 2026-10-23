namespace Infrastructure;

/// <summary>Opções da seção "Messaging". Provider: "Log" (padrão), "RabbitMq" ou "ServiceBus".</summary>
public sealed class MessagingOptions
{
    public const string SectionName = "Messaging";

    public string Provider { get; set; } = "Log";

    public string? ConnectionString { get; set; }
}

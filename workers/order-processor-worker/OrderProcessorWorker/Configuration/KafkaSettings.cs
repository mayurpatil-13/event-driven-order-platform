namespace OrderProcessorWorker.Configuration;

public class KafkaSettings
{
    public string BootstrapServers { get; set; } = string.Empty;
    public string OrderRequestedTopic { get; set; } = string.Empty;
}

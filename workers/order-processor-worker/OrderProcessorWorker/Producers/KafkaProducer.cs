using System.Text.Json;
using Confluent.Kafka;
using OrderProcessorWorker.Models;

namespace OrderProcessorWorker.Producers;
public class KafkaProducer
{
    private readonly IConfiguration _config;

    public KafkaProducer(IConfiguration config)
    {
        _config = config;
    }

    public async Task OrderUpdatedPublishAsync(OrderRequestedEvent orderRequestedEvent, List<ReserveInventoryResponse> reserveInventoryResponse, Boolean isSuccess)
    {
        var producerConfig = new ProducerConfig
        {
            BootstrapServers = _config["Kafka:BootstrapServers"],
            AllowAutoCreateTopics = true,
            // Acks = Acks.All,
            // EnableIdempotence = true,
            // MessageSendMaxRetries = 3,
            // RetryBackoffMs = 1000
        };
        var producer = new ProducerBuilder<Null, string>(producerConfig).Build();
        var topic = _config["Kafka:OrderUpdatedTopic"];
        var orderUpdatedEvent = new OrderUpdateEvent
        {
            OrderId = orderRequestedEvent.OrderId,
            CustomerId = orderRequestedEvent.CustomerId,
            Items = orderRequestedEvent.Items,
            IsSuccess = isSuccess,
            InventoryResults = reserveInventoryResponse
        };
        var message = JsonSerializer.Serialize(orderUpdatedEvent);
        await producer.ProduceAsync(topic, new Message<Null, string>
        {
            Value = message
        });
        Console.WriteLine("📩 OrderUpdated event published: " + message);
    }
}
using System.Text.Json;
using Confluent.Kafka;

namespace OrderService.Kafka
{
    public class KafkaProducer
    {
        private readonly IConfiguration _config;

        public KafkaProducer(IConfiguration config)
        {
            _config = config;
        }

        public async Task PublishOrderCreatedEvent(OrderCreatedEvent orderCreatedEvent)
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
            var topic = _config["Kafka:OrderCreatedTopic"];
            var message = JsonSerializer.Serialize(orderCreatedEvent);
            await producer.ProduceAsync(topic, new Message<Null, string>
            {
                Value = message
            });
            Console.WriteLine("📩 OrderCreated event published: " + message);
        }

        public async Task PublishOrderRequestedEvent(OrderRequestedEvent orderRequestedEvent)
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
            var topic = _config["Kafka:OrderRequestedTopic"];
            var message = JsonSerializer.Serialize(orderRequestedEvent);
            await producer.ProduceAsync(topic, new Message<Null, string>
            {
                Value = message
            });
            Console.WriteLine("📩 OrderRequested event published: " + message);
        }
    }
}

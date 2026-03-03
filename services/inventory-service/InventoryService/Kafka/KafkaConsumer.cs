using System.Text.Json;
using Confluent.Kafka;
using Inventory.Kafka;
using InventoryService.Data;

namespace InventoryService.Kafka
{
    public class KafkaConsumer : BackgroundService
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly IConfiguration _config;

        public KafkaConsumer(IServiceScopeFactory serviceScopeFactory, IConfiguration config)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _config = config;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            return Task.Run(() =>
            {
                var consumerConfig = new ConsumerConfig
                {
                    BootstrapServers = _config["Kafka:BootstrapServers"],
                    GroupId = "inventory-service",
                    AutoOffsetReset = AutoOffsetReset.Earliest
                };


                using var consumer = new ConsumerBuilder<Ignore, string>(consumerConfig).Build();
                consumer.Subscribe(_config["Kafka:OrderCreatedTopic"]);

                while (!stoppingToken.IsCancellationRequested)
                    {
                        var result = consumer.Consume(stoppingToken);

                        var evt = JsonSerializer.Deserialize<OrderCreatedEvent>(result.Message.Value);

                        if (evt == null) continue;

                        using var scope = _serviceScopeFactory.CreateScope();
                        var db = scope.ServiceProvider.GetRequiredService<InventoryDbContext>();

                        foreach (var item in evt.Items)
                        {
                            var product = db.Products.FirstOrDefault(p => p.Id == item.ProductId);

                            if (product == null) continue;

                            product.Stock -= item.Quantity;
                            product.UpdatedAt = DateTime.UtcNow;
                        }

                        db.SaveChanges();
                    }
            }, stoppingToken);
            
        }
    }
}
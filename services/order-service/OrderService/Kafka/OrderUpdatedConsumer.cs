using System.Text.Json;
using Confluent.Kafka;
using OrderProcessorWorker.Models;
using OrderService.Interfaces;

namespace OrderService.Kafka;

public class OrderUpdatedConsumer : BackgroundService
{
    private readonly IConfiguration _config;
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public OrderUpdatedConsumer(IConfiguration config, IServiceScopeFactory serviceScopeFactory)
    {
        _config = config;
        _serviceScopeFactory = serviceScopeFactory;
    }
    
    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        return Task.Run(async() =>
        {
            var consumerConfig = new ConsumerConfig
            {
                BootstrapServers = _config["Kafka:BootstrapServers"],
                GroupId = _config["Kafka:GroupId"],
                AutoOffsetReset = AutoOffsetReset.Earliest
            };

            var consumer = new ConsumerBuilder<Ignore, string>(consumerConfig).Build();
            consumer.Subscribe(_config["Kafka:OrderUpdatedTopic"]);

            while (!stoppingToken.IsCancellationRequested)
            {
                var result = consumer.Consume(stoppingToken);

                var evt = JsonSerializer.Deserialize<OrderUpdateEvent>(result.Message.Value);

                if (evt == null) continue;

                using var scope = _serviceScopeFactory.CreateScope();
                var orderService = scope.ServiceProvider.GetRequiredService<IOrderService>();
                await orderService.UpdateOrderStatus(evt);

                Console.WriteLine("📩 OrderUpdated event consumed: " + result.Message.Value);
            }

        }, stoppingToken);
    }
}
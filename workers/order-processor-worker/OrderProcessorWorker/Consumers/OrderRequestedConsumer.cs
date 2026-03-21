using System.Text.Json;
using System.Text.RegularExpressions;
using Confluent.Kafka;
using OrderProcessorWorker.Models;
using OrderProcessorWorker.Services;

namespace OrderProcessorWorker.Consumers;

public class OrderRequestedConsumer : BackgroundService
{
    private readonly IConfiguration _config;
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public OrderRequestedConsumer(IConfiguration config, IServiceScopeFactory serviceScopeFactory)
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
                GroupId = "order-processor-worker",
                AutoOffsetReset = AutoOffsetReset.Earliest
            };

            var consumer = new ConsumerBuilder<Ignore, string>(consumerConfig).Build();
            consumer.Subscribe(_config["Kafka:OrderRequestedTopic"]);

            while (!stoppingToken.IsCancellationRequested)
            {
                var result = consumer.Consume(stoppingToken);

                Console.WriteLine("📩 OrderRequested event received: " + result.Message.Value);

                var evt = JsonSerializer.Deserialize<OrderRequestedEvent>(result.Message.Value);

                if (evt == null) continue;

                using var scope = _serviceScopeFactory.CreateScope();
                var orderProcessingService = scope.ServiceProvider.GetRequiredService<OrderProcessingService>();

                await orderProcessingService.ProcessOrderAsync(evt);
            }

        }, stoppingToken);
    }
}
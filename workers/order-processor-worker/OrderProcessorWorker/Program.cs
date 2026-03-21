using OrderProcessorWorker;
using OrderProcessorWorker.Clients;
using OrderProcessorWorker.Consumers;
using OrderProcessorWorker.Producers;
using OrderProcessorWorker.Services;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<Worker>();
builder.Services.AddHostedService<OrderRequestedConsumer>();
builder.Services.AddScoped<OrderProcessingService>();
builder.Services.AddHttpClient<InventoryClient>(client =>
{
    var baseUrl = builder.Configuration["InventoryService:BaseUrl"] ?? throw new InvalidOperationException("InventoryService:BaseUrl is not configured.");
    client.BaseAddress = new Uri(baseUrl);
});
builder.Services.AddScoped<KafkaProducer>();


var host = builder.Build();
host.Run();

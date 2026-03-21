namespace OrderProcessorWorker.Services;

using OrderProcessorWorker.Models;
using OrderProcessorWorker.Clients;
using OrderProcessorWorker.Producers;

public class OrderProcessingService
{
    private readonly InventoryClient _inventoryClient;
    private readonly KafkaProducer _kafkaProducer;

    public OrderProcessingService(InventoryClient inventoryClient, KafkaProducer kafkaProducer)
    {
        _inventoryClient = inventoryClient;
        _kafkaProducer = kafkaProducer;
    }

    public async Task ProcessOrderAsync(OrderRequestedEvent orderEvent)
    {
       var reservationResults = await _inventoryClient.ReserveInventoryAsync(new ReserveInventoryRequest
       {
            OrderId = orderEvent.OrderId,
            Items = [.. orderEvent.Items.Select(item => new InventoryItem
            {
                ProductId = item.ProductId,
                Quantity = item.Quantity
            })]
       });

       reservationResults.ForEach(result =>
       {
           if (!result.Success)
           {
               Console.WriteLine($"Failed to reserve inventory for ProductId: {result.ProductId}");
           }
       });

         // Publish OrderUpdated event with reservation results
        await _kafkaProducer.OrderUpdatedPublishAsync(orderEvent, reservationResults, reservationResults.All(r => r.Success));

    }
}
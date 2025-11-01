using System;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using orders.projection.worker.Adapters.Infra.Database.Options;
using orders.projection.worker.Core.Events;
using orders.projection.worker.Core.Ports.Repositories;
using orders.projection.worker.Core.Ports.Repositories.Projections;
using orders.projection.worker.Core.Projections.OrderSummary;

namespace orders.projection.worker.Adapters.Infra.Repositories;

public class OrderSummaryRepository : IOrderSummaryRepository
{
    private readonly IMongoCollection<OrderSummary> _orderSummary;
    private readonly IMongoCollection<Product> _product;
    private readonly IMongoCollection<Customer> _customer;
    public OrderSummaryRepository(IOptions<MongoDbOptions> options)
    {
        var client = new MongoClient(options.Value.ConnectionString);
        var database = client.GetDatabase(options.Value.DatabaseName);
        _orderSummary = database.GetCollection<OrderSummary>(nameof(OrderSummary));
        _product = database.GetCollection<Product>(nameof(Product));
        _customer = database.GetCollection<Customer>(nameof(Customer));
    }
    public async Task HandleOrderCreatedAsync(OrderCreatedEvent @event)
    {
        var filter = Builders<OrderSummary>.Filter
            .Eq(p => p.OrderId, @event.OrderId);

        var customerRef = await _customer.Find(p=>p.CustomerId.Equals(@event.CustomerId)).FirstOrDefaultAsync();

        var update = Builders<OrderSummary>.Update
            .Set(p => p.Customer, customerRef)
            .Push(p => p.Status, new OrderStatus()
            {
                Status = "Pending",
                Timestamp = @event.Timestamp
            });
            
        await _orderSummary.UpdateOneAsync(
            filter: filter,
            update: update,
            options: new UpdateOptions { IsUpsert = true }
        );
    }

    public async Task HandleOrderItemAddedAsync(OrderItemAddedEvent @event)
    {
        var filter = Builders<OrderSummary>.Filter
            .Eq(p => p.OrderId, @event.OrderId);

        var product = await _product.Find(p => p.ProductId.Equals(@event.ProductId)).FirstOrDefaultAsync();

        var item = new OrderItem()
        {
            ProductId = product.ProductId,
            ProductName = product.ProductName,
            Quantity = @event.Quantity,
            Price = product.Price
        };

        var update = Builders<OrderSummary>.Update.Push(p => p.Items, item);

        await _orderSummary.UpdateOneAsync(
            filter,
            update,
            new UpdateOptions { IsUpsert = true }
        );
    }

    public async Task HandleOrderPaidAsync(OrderPaidEvent @event)
    {
        var filter = Builders<OrderSummary>.Filter
            .Eq(p => p.OrderId, @event.OrderId);

        var update = Builders<OrderSummary>.Update.Push(p => p.Status, new OrderStatus()
        {
            Status = "Paid",
            Timestamp = @event.Timestamp
        });

        await _orderSummary.UpdateOneAsync(
            filter,
            update,
            new UpdateOptions { IsUpsert = true }
        );
    }
}

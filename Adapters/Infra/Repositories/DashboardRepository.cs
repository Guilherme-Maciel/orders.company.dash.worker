using Microsoft.Extensions.Options;
using MongoDB.Driver;
using orders.company.projection.worker.Adapters.Infra.Database.Options;
using orders.company.projection.worker.Core.Events;
using orders.company.projection.worker.Core.Ports.Repositories;
using orders.company.projection.worker.Core.Projections;
using orders.company.projection.worker.Core.Projections.OrderSummary;

namespace orders.company.projection.worker.Adapters.Infra.Repositories;

public class DashboardRepository : IDashboardRepository
{
    private readonly IMongoCollection<DashboardStats> _dashStats;

    public DashboardRepository(IOptions<MongoDbOptions> options)
    {
        var client = new MongoClient(options.Value.ConnectionString);
        var database = client.GetDatabase(options.Value.DatabaseName);
        _dashStats = database.GetCollection<DashboardStats>(nameof(DashboardStats));
    }
    public async Task HandleOrderCreatedAsync(OrderCreatedEvent @event)
    {
        var dashId = $"dashboard_stats_{@event.Timestamp:yyyy_MM_dd}";

        var filter = Builders<DashboardStats>.Filter
            .Eq(p => p.Id, dashId);

        var update = Builders<DashboardStats>.Update
            .Inc(p => p.PendingOrders, 1);

        await _dashStats.UpdateOneAsync(
            filter: filter,
            update: update,
            options: new UpdateOptions { IsUpsert = true }
        );
    }

    public async Task HandleOrderPaidAsync(OrderPaidEvent @event)
    {
        var dashId = $"dashboard_stats_{@event.Timestamp:yyyy_MM_dd}";

        var filter = Builders<DashboardStats>.Filter
            .Eq(p => p.Id, dashId);

        var update = Builders<DashboardStats>.Update
            .Inc(p => p.PaidOrders, 1)
            .Inc(p => p.TotalRevenue, @event.Total);

        await _dashStats.UpdateOneAsync(
            filter: filter,
            update: update,
            options: new UpdateOptions { IsUpsert = true }
        );
    }
}

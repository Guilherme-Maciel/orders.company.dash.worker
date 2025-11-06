using System;
using orders.company.projection.worker.Core.Events;

namespace orders.company.projection.worker.Core.Ports.Repositories;

public interface IDashboardRepository
{
    Task HandleOrderCreatedAsync(OrderCreatedEvent @event);
    Task HandleOrderPaidAsync(OrderPaidEvent @event);
}

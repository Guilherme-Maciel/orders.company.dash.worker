using System;

namespace orders.company.projection.worker.Core.Events;

public class OrderPaidEvent
{
    public Guid OrderId { get; set; }
    public Guid CustomerId { get; set; }
    public DateTime Timestamp { get; set; }
    public decimal Total { get; set; }
}

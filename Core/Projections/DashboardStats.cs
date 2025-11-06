using MongoDB.Bson.Serialization.Attributes;

namespace orders.company.projection.worker.Core.Projections
{
    public class DashboardStats
    {
        [BsonId]
        [BsonRepresentation(MongoDB.Bson.BsonType.String)]
        public string Id { get; set; }
        public decimal TotalRevenue { get; set; }
        public int PendingOrders { get; set; }
        public int PaidOrders { get; set; }
    }
}

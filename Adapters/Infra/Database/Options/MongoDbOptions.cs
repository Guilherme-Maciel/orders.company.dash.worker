using System;

namespace orders.company.projection.worker.Adapters.Infra.Database.Options;

public class MongoDbOptions
{
    public string DatabaseName { get; set; }
    public string ConnectionString { get; set; }
}

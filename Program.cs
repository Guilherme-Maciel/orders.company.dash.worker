using Microsoft.Extensions.Options;
using orders.company.projection.worker;
using orders.company.projection.worker.Adapters.Infra.Database.Options;
using orders.company.projection.worker.Adapters.Infra.Messaging.Options;
using orders.company.projection.worker.Adapters.Infra.Repositories;
using orders.company.projection.worker.Core.Ports.Repositories;
using RabbitMQ.Client;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.Configure<MongoDbOptions>(builder.Configuration.GetSection("MongoDb"));
builder.Services.Configure<RabbitMqOptions>(builder.Configuration.GetSection("RabbitMq"));

builder.Services.AddSingleton<IDashboardRepository, DashboardRepository>();

builder.Services.AddSingleton(sp =>
{
    var options = sp.GetRequiredService<IOptions<RabbitMqOptions>>().Value;
    var factory = new ConnectionFactory
    {
        HostName = options.HostName,
        UserName = options.UserName,
        Password = options.Password,
        AutomaticRecoveryEnabled = true
    };
    return factory.CreateConnectionAsync().GetAwaiter().GetResult();
});

builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();

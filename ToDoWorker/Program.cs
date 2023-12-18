using MediatR;
using ToDoWorker;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddMediatR(typeof(Program).Assembly);
        services.AddHostedService<Worker>();
        services.AddSingleton<RabbitMQConsumer>();
    })
    .Build();

host.Run();

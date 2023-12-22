using MediatR;
using ToDoWorker;

var host = Host.CreateDefaultBuilder(args)
            .ConfigureServices((hostContext, services) =>
            {
                services.AddMediatR(typeof(Program).Assembly);
                services.AddHttpClient();

                // Register services with appropriate lifetimes
                services.AddTransient<BadgeGenerator>();
                services.AddTransient<RabbitMQConsumer>();
                services.AddTransient<RabbitMqPublisher>(provider =>
                {
                    var configuration = hostContext.Configuration;
                    var hostName = configuration["RabbitMq:HostName"];
                    var exchangeName = configuration["RabbitMq:ExchangeName"];
                    return new RabbitMqPublisher(hostName, exchangeName);
                });

                // Register the Worker as a scoped service
                services.AddHostedService<Worker>();
            })
            .Build();

host.Run();

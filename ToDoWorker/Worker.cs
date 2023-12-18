using MediatR;

namespace ToDoWorker;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly IMediator _mediator;
    private readonly RabbitMQConsumer _rabbitMQConsumer;

    public Worker(ILogger<Worker> logger, IMediator mediator, RabbitMQConsumer rabbitMQConsumer)
    {
        _logger = logger;
        _mediator = mediator;
        _rabbitMQConsumer = rabbitMQConsumer;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // Start RabbitMQConsumer
        _rabbitMQConsumer.StartListening();

        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
            await Task.Delay(1000, stoppingToken);
        }
    }
}

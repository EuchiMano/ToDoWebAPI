namespace ToDoWorker;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly RabbitMQConsumer _rabbitMQConsumer;

    public Worker(ILogger<Worker> logger, RabbitMQConsumer rabbitMQConsumer)
    {
        _logger = logger;
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

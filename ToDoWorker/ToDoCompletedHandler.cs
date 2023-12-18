using MediatR;

namespace ToDoWorker
{
    public class ToDoCompletedHandler : INotificationHandler<ToDoCompletedEvent>
    {
        private readonly ILogger<ToDoCompletedHandler> _logger;
        public ToDoCompletedHandler(ILogger<ToDoCompletedHandler> logger)
        {
            _logger = logger;
        }

        public Task Handle(ToDoCompletedEvent notification, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Todo completed with ID: {notification.Id}");
            return Task.CompletedTask;
        }
    }
}
using MediatR;

namespace ToDoWorker
{
    public class ToDoCompletedEvent : INotification
    {
        public string Id { get; init; }
    }
}
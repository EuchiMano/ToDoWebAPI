namespace ToDoWebAPI.Dtos
{
    public class TodoGetResponse
    {
        public Guid Id { get; set; }
        public string Description { get; set; }
        public DateTime CreationDate { get; set; }
        public bool IsCompleted { get; set; }
        public DateTime? CompletedDate { get; set; }
        public TimeSpan? CompletedDuration { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletedDate { get; set; }
        public TodoInfoResponse Info { get; set; }
    }
}
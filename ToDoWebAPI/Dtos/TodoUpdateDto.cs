namespace ToDoWebAPI.Dtos
{
    public class TodoUpdateDto
    {
        public string Description { get; set; }
        public DateTime CreationDate { get; set; }
        public bool IsCompleted { get; set; }
        public DateTime? CompletedDate { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletedDate { get; set; }
    }
}
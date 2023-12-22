namespace ToDoWebAPI.Dtos
{
    public class TodoPostDto
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Description { get; set; }
        public DateTime CreationDate { get; set; } = DateTime.Now;
        public bool IsCompleted { get; set; }
    }
}
namespace ToDoWebAPI.Models;

public class TodoItem
{
    public string Id { get; set; }
    public string ToDo { get; set; }
    public string Description { get; set; }
    public DateTime Date { get; set; }
    public string Notes { get; set; }
    public string Attachments { get; set; }
}
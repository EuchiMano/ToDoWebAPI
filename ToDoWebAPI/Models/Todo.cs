using System;
namespace ToDoWebAPI.Models;

public class Todo
{
    public Guid Id { get; set; }
    public string Description { get; set; }
    public DateTime CreationDate { get; set; }
    public bool IsCompleted { get; set; }
    public DateTime? CompletedDate { get; set; }
    public TimeSpan? CompletedDuration { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime? DeletedDate { get; set; }
    public TodoInfo Info { get; set; }

    public void CheckIfIsCompleted(bool isCompleted)
    {
        IsCompleted = isCompleted;
        if (IsCompleted)
        {
            CompletedDate = DateTime.UtcNow;
            CompletedDuration = CompletedDate - CreationDate;
        }
        else
        {
            CompletedDate = null;
            CompletedDuration = null;
        }
    }

    public void CreateNewTodo(string description)
    {
        Id = Guid.NewGuid();
        Description = description;
        CreationDate = DateTime.UtcNow;
    }
}

public class TodoInfo
{
    public string BadgePath { get; set; }
    public string ReportPath { get; set; }
}
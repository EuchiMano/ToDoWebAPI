using Microsoft.EntityFrameworkCore;

namespace ToDoWebAPI.Models;

public class TodoDbContext : DbContext
{
    public TodoDbContext(DbContextOptions<TodoDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Todo>()
            .OwnsOne(t => t.Info);

        base.OnModelCreating(modelBuilder);
    }

    public DbSet<Todo> Todos { get; set; }
}
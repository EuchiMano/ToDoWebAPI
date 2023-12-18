using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RabbitMQ.Client;
using ToDoWebAPI.Models;

namespace ToDoWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TodoController : ControllerBase
    {
        private readonly TodoDbContext _todoDbContext;

        public TodoController(TodoDbContext todoDbContext)
        {
            _todoDbContext = todoDbContext;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllTodos()
        {
            var todos = await _todoDbContext.Todos
                .Where(x => x.IsDeleted == false)
                .OrderByDescending(x => x.CreationDate)
                .ToListAsync();
            return Ok(todos);
        }

        [HttpPost]
        public async Task<IActionResult> AddTodo(Todo todo)
        {
            todo.Id = Guid.NewGuid();
            _todoDbContext.Todos.Add(todo);
            await _todoDbContext.SaveChangesAsync();

            return Ok(todo);
        }

        [HttpPut]
        [Route("{id:Guid}")]
        public async Task<IActionResult> UpdateTodo([FromRoute] Guid id,
        Todo todoUpdateRequest, [FromServices] IModel rabbitMQChannel)
        {
            var todo = await _todoDbContext.Todos.FindAsync(id);
            if (todo is null) return NotFound();
            todo.IsCompleted = todoUpdateRequest.IsCompleted;
            todo.CompletedDate = DateTime.Now;
            await _todoDbContext.SaveChangesAsync();

            if (todoUpdateRequest.IsCompleted)
            {
                // Convert your message to a byte array (example assumes message is a string)
                var messageBytes = Encoding.UTF8.GetBytes($"TodoCompletedEvent: {id}");

                // Publish the message to the exchange
                rabbitMQChannel.BasicPublish(exchange: "my_direct_exchange",
                                             routingKey: "",
                                             basicProperties: null,
                                             body: messageBytes);
            }

            return Ok(todo);
        }

        [HttpDelete]
        [Route("{id:Guid}")]
        public async Task<IActionResult> DeleteTodo([FromRoute] Guid id)
        {
            var todo = await _todoDbContext.Todos.FindAsync(id);
            if (todo is null) return NotFound();
            todo.IsDeleted = true;
            todo.DeletedDate = DateTime.Now;

            await _todoDbContext.SaveChangesAsync();
            return Ok(todo);
        }

        [HttpGet]
        [Route("get-deleted-todos")]
        public async Task<IActionResult> GetDeletedTodos()
        {
            var todos = await _todoDbContext.Todos
                .Where(x => x.IsDeleted == true)
                .OrderByDescending(x => x.CreationDate)
                .ToListAsync();
            return Ok(todos);
        }

        [HttpPut]
        [Route("undo-deleted-todo/{id:Guid}")]
        public async Task<IActionResult> UndoDeletedTodo([FromRoute] Guid id, Todo undoDeleteRequest)
        {
            var todo = await _todoDbContext.Todos.FindAsync(id);
            if (todo is null) return NotFound();
            todo.DeletedDate = null;
            todo.IsDeleted = false;
            await _todoDbContext.SaveChangesAsync();
            return Ok(todo);
        }
    }
}

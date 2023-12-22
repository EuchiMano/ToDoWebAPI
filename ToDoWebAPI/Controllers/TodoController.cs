using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using RabbitMQ.Client;
using ToDoWebAPI.Dtos;
using ToDoWebAPI.Messages;
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
        public async Task<IActionResult> AddTodo(TodoPostDto todoPost)
        {
            var newTodo = new Todo{
                Id = todoPost.Id,
                Description = todoPost.Description,
                CreationDate = todoPost.CreationDate,
                IsCompleted = todoPost.IsCompleted
            };
            _todoDbContext.Todos.Add(newTodo);
            await _todoDbContext.SaveChangesAsync();
            return Ok(todoPost);
        }

        [HttpPut]
        [Route("{id:Guid}")]
        public async Task<IActionResult> UpdateTodo([FromRoute] Guid id,
        TodoUpdateDto todoUpdateRequest, [FromServices] IModel rabbitMQChannel)
        {
            var todo = await _todoDbContext.Todos.FindAsync(id);
            if (todo is null) return NotFound();
            todo.CheckIfIsCompleted(todoUpdateRequest.IsCompleted);
            await _todoDbContext.SaveChangesAsync();

            if (todoUpdateRequest.IsCompleted)
            {
                var todoCompletedEvent = new TodoCompletedMessage
                {
                    TodoId = id
                };

                // Serialize the message to a byte array (example assumes JSON serialization)
                var messageBytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(todoCompletedEvent));

                // Publish the message to the exchange
                rabbitMQChannel.BasicPublish(exchange: "my_direct_exchange",
                                             routingKey: "",
                                             basicProperties: null,
                                             body: messageBytes);
            }

            return Ok(todo);
        }

        [HttpPut]
        [Route("update-badge")]
        public async Task<IActionResult> UpdateTodoBadge(UpdateTodoBadgeDto todoUpdateRequest)
        {
            var todo = await _todoDbContext.Todos.FindAsync(todoUpdateRequest.TodoId);

            if (todo == null)
            {
                return NotFound();
            }

            todo.Info.BadgePath = todoUpdateRequest.BadgeUrl;
            await _todoDbContext.SaveChangesAsync();
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

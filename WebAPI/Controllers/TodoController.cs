using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using TodoApi.Models;
using TodoApi.Services;

[ApiController]
[Route("api/todo")]
public class TodoController : ControllerBase
{
    private readonly ITodoService _todoService;

    public TodoController(ITodoService todoService)
    {
        _todoService = todoService;
    }
    // GET: api/todo
    [HttpGet]
    public async Task<ActionResult<IEnumerable<TodoItem>>> GetTodos()
    {
        return await _todoService.GetTodos();
    }

    // POST: api/todo
    [HttpPost]
    public async Task<ActionResult<TodoItem>> AddTodo(TodoItem item)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        var todo = await _todoService.AddTodo(item);
        return CreatedAtAction(nameof(GetTodos), new { id = todo.Id }, todo);
    }

    // PUT: api/todo/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> PutTodoItem(int id, TodoDto item)
    {
        if (id != item.Id)
        {
            return BadRequest();
        }
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        var existingTodo = await _todoService.UpdateTodo(item);
        if (existingTodo == null)
        {
            return NotFound();
        }
        return Ok("Update Successfully.");
    }

    // DELETE: api/todo/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTodo(int id)
    {
        await _todoService.DeleteTodoById(id);
        return NoContent();
    }
}

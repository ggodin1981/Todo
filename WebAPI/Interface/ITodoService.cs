using System.Collections.Generic;
using System.Threading.Tasks;
using TodoApi.Models;

namespace TodoApi.Services
{
    public interface ITodoService
    {
        Task<List<TodoItem>> GetTodos();
        Task<TodoItem> GetTodoById(int id);
        Task<TodoItem> AddTodo(TodoItem item);
        Task<TodoItem> UpdateTodo(TodoDto item);      
        Task DeleteTodoById(int id);
        bool TodoItemExists(int id);
    }
}

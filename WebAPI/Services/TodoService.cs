using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TodoApi.Data;
using TodoApi.Models;

namespace TodoApi.Services
{
    public class TodoService : ITodoService
    {
        private readonly TodoContext _context;

        public TodoService(TodoContext context)
        {
            _context = context;
        }

        public async Task<List<TodoItem>> GetTodos()
        {
            return await _context.TodoItems.ToListAsync();
        }

        public async Task<TodoItem> GetTodoById(int id)
        {
            return await _context.TodoItems.FindAsync(id);
        }

        public async Task<TodoItem> AddTodo(TodoItem item)
        {
            _context.TodoItems.Add(item);
            await _context.SaveChangesAsync();
            return item;
        }

        public async Task<TodoItem> UpdateTodo(TodoDto item)
        {
            var existingTodo = await _context.TodoItems.FindAsync(item.Id);
            if (existingTodo != null)
            {
                existingTodo.IsCompleted = !existingTodo.IsCompleted;
                await _context.SaveChangesAsync();
            }
            return existingTodo;
        }        

        public async Task DeleteTodoById(int id)
        {
            var todoItem = await _context.TodoItems.FindAsync(id);             
            if (todoItem != null)
            {
                _context.TodoItems.Remove(todoItem);
                await _context.SaveChangesAsync();
            }
        }

        public bool TodoItemExists(int id)
        {
            return _context.TodoItems.Any(e => e.Id == id);
        }
    }
}

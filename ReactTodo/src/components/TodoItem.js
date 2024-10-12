import React from 'react';
import todoService from '../services/todoService';

function TodoItem({ todo, fetchTodos }) {
    function toggleTodo() {
        todoService.toggleTodo(todo.id).then(fetchTodos);
    }

    function deleteTodo() {
        todoService.deleteTodo(todo.id).then(fetchTodos);
    }

    return (
        <li>
            <span
                style={{
                    textDecoration: todo.isCompleted ? 'line-through' : 'none',
                }}
            >
                {todo.title}
            </span>
            <button onClick={toggleTodo}>Toggle</button>
            <button onClick={deleteTodo}>Delete</button>
        </li>
    );
}

export default TodoItem;

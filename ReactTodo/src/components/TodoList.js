import React from 'react';
import TodoItem from './TodoItem';

function TodoList({ todos, fetchTodos }) {
    return (
        <ul>
            {todos.map(todo => (
                <TodoItem key={todo.id} todo={todo} fetchTodos={fetchTodos} />
            ))}
        </ul>
    );
}

export default TodoList;

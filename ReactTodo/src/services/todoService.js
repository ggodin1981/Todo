const API_URL = 'https://localhost:5000/api/todo';

// Fetch todos from the API
export async function getTodos() {
    try {
        const response = await fetch(API_URL);
        if (!response.ok) {
            throw new Error('Failed to fetch todos');
        }
        return await response.json();
    } catch (error) {
        throw error; // Throw the error to be handled in the component
    }
}

// Create a new todo
export async function createTodo(title) {
    if (!title.trim()) return; // Prevent empty todos
    try {
        const response = await fetch(API_URL, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
            },
            body: JSON.stringify({ title }),
        });
        if (!response.ok) {
            throw new Error('Failed to add todo');
        }
    } catch (error) {
        throw error; // Throw the error to be handled in the component
    }
}

// Toggle a todo's completion status
export async function toggleTodo(id, isCompleted) {
    try {
        const response = await fetch(`${API_URL}/${id}`, {
            method: 'PUT',
            headers: {
                'Content-Type': 'application/json',
            },
            body: JSON.stringify({ 
                id:id,
                isCompleted: !isCompleted
            }),
        });
        if (!response.ok) {
            throw new Error('Failed to update todo');
        }
    } catch (error) {
        throw error; // Throw the error to be handled in the component
    }
}

// Delete a todo
export async function deleteTodo(id) {
    try {
        const response = await fetch(`${API_URL}/${id}`, { method: 'DELETE' });
        if (!response.ok) {
            throw new Error('Failed to delete todo');
        }        
    } catch (error) {
        throw error; // Throw the error to be handled in the component
    }
}

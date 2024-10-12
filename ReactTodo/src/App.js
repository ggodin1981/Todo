import React, { useState, useEffect } from 'react';
import { getTodos as fetchTodos, createTodo, toggleTodo, deleteTodo } from './services/todoService'; // Import API functions from your service

const App = () => {
    // Define state for todos
    const [todos, setTodos] = useState([]);
    
    // Define state for input title
    const [title, setTitle] = useState('');
    
    // Define state for errors
    const [error, setError] = useState(null);

    // Use useEffect to fetch todos on component mount
    useEffect(() => {
        fetchTodosFromService();
    }, []);

    const sanitizeInput = (input) => {
        const sanitizedInput = input.replace(/<\/?[^>]+(>|$)/g, ""); // Remove HTML tags
        return sanitizedInput.trim(); // Remove extra spaces
    };

    const validateInput = (input) => {
        if (input.length === 0) {
            setError("Todo title cannot be empty.");
            return false;
        }
        if (input.length > 100) { // Example: limit length to 100 characters
            setError("Todo title cannot be longer than 100 characters.");
            return false;
        }
        return true;
    };

    // Function to fetch todos from the service
    const fetchTodosFromService = async () => {
        try {
            const todosData = await fetchTodos(); // Call the service
            setTodos(todosData); // Update state with fetched todos
        } catch (err) {
            setError(err.message); // Set error state
        }
    };

    // Function to handle creating a new todo
    const addTodo = async () => {
        if (title.trim()) { // Check if title is not empty
            try {
                const sanitizedTitle = sanitizeInput(title); // Sanitize the input
                if (!validateInput(sanitizedTitle)) {
                    return; // Stop if validation fails
                }                
                await createTodo(sanitizedTitle); // Call service to create a new todo
                setTitle(''); // Clear input field
                fetchTodosFromService(); // Refresh todos
            } catch (err) {
                setError(err.message); // Handle error
            }
        }
        else{
            setError('title is required'); // Handle error
        }
    };     
    const deletetodo = async (id) => {
        try {
            await deleteTodo(id); 
            setTitle(''); // Clear input field
            fetchTodosFromService(); // Refresh todos
        } catch (err) {
            setError(err.message); // Handle error
        }
    };
    const toggletodo = async (id,isCompleted) => {
        try {
            await toggleTodo(id,isCompleted);            
            fetchTodosFromService(); // Refresh todos
        } catch (err) {
            setError(err.message); // Handle error
        }
    };

    return (
            <div>
            <h1>Todo List</h1>
            {error && <p style={{ color: 'red' }}>{error}</p>}
            <input type="text" value={title} maxlength="50" onChange={(e) => {setError(null); // Clear error on new input
                    setTitle(e.target.value);}}
                placeholder="Add a new todo"
            />
            <button onClick={addTodo}>Add</button>
            <ul>
                {todos.map(todo => (
                    <li key={todo.id}>
                        <span
                            style={{
                                textDecoration: todo.isCompleted ? 'line-through' : 'none'
                            }}
                        >
                            {todo.title}
                        </span>
                        <button onClick={() => toggletodo(todo.id,todo.isCompleted)}>Toggle</button>
                        <button onClick={() => deletetodo(todo.id)}>Delete</button>
                    </li>
                ))}
            </ul>
        </div>
    );
};

export default App;

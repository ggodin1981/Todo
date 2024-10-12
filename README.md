## **Task Description**
Below is the source code for the simple To-Do List application that requires refactoring. The code intentionally includes various issues related to code quality, organization, and best practices as described in the task. Your goal is to refactor this code to improve its overall quality.

---

## **Backend: ASP.NET Core Web API (C#)**

### **Program.cs**

```csharp
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapControllers();

app.Run();
```

### **TodoItem.cs**

```csharp
public class TodoItem
{
    public int Id { get; set; }
    public string Title { get; set; }
    public bool IsCompleted { get; set; }
}
```

### **TodoController.cs**

```csharp
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;

[ApiController]
[Route("api/todo")]
public class TodoController : ControllerBase
{
    public static List<TodoItem> todos = new List<TodoItem>();
    public int nextId = 1;

    [HttpGet]
    public IEnumerable<TodoItem> GetTodos()
    {
        return todos;
    }

    [HttpPost]
    public void AddTodo(TodoItem item)
    {
        item.Id = nextId;
        nextId++;
        todos.Add(item);
    }

    [HttpPut("{id}")]
    public void UpdateTodo(int id)
    {
        var todo = todos.FirstOrDefault(x => x.Id == id);
        if (todo != null)
        {
            todo.IsCompleted = !todo.IsCompleted;
        }
    }

    [HttpDelete]
    public void DeleteTodo(int id)
    {
        var todo = todos.FirstOrDefault(x => x.Id == id);
        if (todo != null)
        {
            todos.Remove(todo);
        }
    }
}
```

---

## **Frontend: React**

### **index.js**

```javascript
import React from 'react';
import ReactDOM from 'react-dom';
import App from './App.js';

ReactDOM.render(<App />, document.getElementById('root'));
```

### **App.js**

```javascript
import React, { useState, useEffect } from 'react';

function App() {
    const [todos, setTodos] = useState([]);
    const [title, setTitle] = useState('');

    function fetchTodos() {
        fetch('http://localhost:5000/api/todo')
            .then(response => response.json())
            .then(data => setTodos(data));
    }

    useEffect(() => {
        fetchTodos();
    }, []);

    function addTodo() {
        fetch('http://localhost:5000/api/todo', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({ title: title })
        }).then(() => {
            fetchTodos();
            setTitle('');
        });
    }

    function toggleTodo(id) {
        fetch(`http://localhost:5000/api/todo/${id}`, {
            method: 'PUT'
        }).then(() => {
            fetchTodos();
        });
    }

    function deleteTodo(id) {
        fetch('http://localhost:5000/api/todo', {
            method: 'DELETE',
            body: id
        }).then(() => {
            fetchTodos();
        });
    }

    return (
        <div>
            <h1>Todo List</h1>
            <input value={title} onChange={e => setTitle(e.target.value)} />
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
                        <button onClick={() => toggleTodo(todo.id)}>Toggle</button>
                        <button onClick={() => deleteTodo(todo.id)}>Delete</button>
                    </li>
                ))}
            </ul>
        </div>
    );
}

export default App;
```

### **package.json**

```json
{
  "name": "todo-app",
  "version": "0.1.0",
  "private": true,
  "dependencies": {
    "react": "^17.0.1",
    "react-dom": "^17.0.1"
  },
  "scripts": {
    "start": "react-scripts start"
  }
}
```

---

## **Instructions**

1. **Set Up the Backend**

   - Create a new ASP.NET Core Web API project.
   - Replace the content with the provided backend code.
   - Run the application. It should listen on `http://localhost:5000`.

2. **Set Up the Frontend**

   - Create a new React application using Create React App.
   - Replace the content with the provided frontend code.
   - Run the application using `npm start` or `yarn start`.

3. **Verify Functionality**

   - Test the application to understand how it currently works.
   - Note down any bugs or issues you encounter.

4. **Begin Refactoring**

   - Follow the guidelines provided in the task description to refactor both the backend and frontend code.

---

## **Additional Notes**

- **Assumptions**

  - The backend and frontend are running on the same machine.
  - No database is required; data is stored in-memory.
  - The focus is on code quality and best practices rather than adding new features.

- **Points to Consider**

  - **Security:** Sanitize and validate all user inputs.
  - **Scalability:** Consider thread-safety and state management in the backend.
  - **Maintainability:** Organize code into modules and use meaningful naming conventions.
  - **Performance:** Optimize API calls and rendering in the frontend.

- **Submission Instructions**
  - Create a new repository on your personal GitHub account.
  - Commit your changes and ensure your code is well-documented.
  - Share the repository link once you are done.

---

**Good Luck with the Refactoring Task!**

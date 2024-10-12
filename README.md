---

## **Backend: ASP.NET Core Web API (C#)**

1. **Security: Sanitize and Validate User Inputs:**
 - Input validation is critical to prevent malicious data from entering the system (e.g., SQL Injection, Cross-Site Scripting). ASP.NET Core provides built-in validation mechanisms.
    - Model Validation: Using data annotations on the model.
    - Input Sanitization: Using anti-XSS techniques.
    - Client-Side Validation: Ensures that forms are validated on the client-side before sending to the server.
      
Update TodoItem.cs to include proper validation:

```csharp
using System.ComponentModel.DataAnnotations;
namespace TodoApi.Models
{
    public class TodoItem
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Title is required.")]
        [StringLength(100, ErrorMessage = "Title length cannot exceed 100 characters.")]
        public string Title { get; set; }

        public bool IsCompleted { get; set; } = false;
    }
}
```
In the controller, ensure the model state is validated:

```csharp
[HttpPost]
public async Task<ActionResult<TodoItem>> AddTodo([FromBody] TodoItem item)
{
    if (!ModelState.IsValid)
    {
        return BadRequest(ModelState);
    }
    // Additional input sanitization can be done if needed.
    _context.TodoItems.Add(item);
    await _context.SaveChangesAsync();
    return CreatedAtAction(nameof(GetTodos), new { id = item.Id }, item);
}

[HttpPut("{id}")]
public async Task<IActionResult> UpdateTodo(int id, [FromBody] TodoItem item)
{
    if (id != item.Id)
    {
        return BadRequest("ID mismatch");
    }
    if (!ModelState.IsValid)
    {
        return BadRequest(ModelState);
    }
    _context.Entry(item).State = EntityState.Modified;
    try
    {
        await _context.SaveChangesAsync();
    }
    catch (DbUpdateConcurrencyException)
    {
        if (!TodoItemExists(id))
        {
            return NotFound();
        }
        else
        {
            throw;
        }
    }
    return NoContent();
}
```

2. **Scalability: Thread-Safety and State Management**
In-memory databases are not shared across multiple instances of an application, and EF Core takes care of thread safety for you. However, to scale across instances, a persistent database like SQL Server or Redis is better.

For this in-memory example, no additional thread safety measures are needed since EF Core handles concurrent requests. But if you were to persist state or scale further, you'd need a distributed system (e.g., Redis for session state management or scaling EF Core with an actual database).

**Thread Safety:**
 - EF Core handles thread safety internally, so we don’t need to manually lock resources.

3. **Maintainability: Code Organization and Naming Conventions**
   
 - We should apply clean architecture principles such as separating concerns by organizing code into appropriate folders for Controllers, Models, Services, and Data layers. Here's how to organize the project:
   
```
TodoApi/
├── Properties/
│   └──launchSettings.json
├── Controllers/
│   └── TodoController.cs
├── Data/
│   └── TodoContext.cs
├── Models/
│   └── TodoItem.cs
├── Interface/
│   └── ITodoService.cs
├── Services/│   
│   └── TodoService.cs
├── Program.cs

```
Now, let’s introduce a service layer for better separation of concerns. The service will handle business logic, and the controller will focus on routing.


**Properties**

**launchSettings**

```json
{
  "$schema": "http://json.schemastore.org/launchsettings.json",
  "iisSettings": {
    "windowsAuthentication": false,
    "anonymousAuthentication": true,
    "iisExpress": {
      "applicationUrl": "http://localhost:57828",
      "sslPort": 44389
    }
  },
  "profiles": {
    "http": {
      "commandName": "Project",
      "dotnetRunMessages": true,
      "launchBrowser": true,
      "launchUrl": "swagger",
      "applicationUrl": "http://localhost:5000",
      "environmentVariables": {
        "ASPNETCORE_ENVIRONMENT": "Development"
      }
    },
    "https": {
      "commandName": "Project",
      "dotnetRunMessages": true,
      "launchBrowser": true,
      "launchUrl": "swagger",
      "applicationUrl": "https://localhost:5000;http://localhost:7180",
      "environmentVariables": {
        "ASPNETCORE_ENVIRONMENT": "Development"
      }
    },
    "IIS Express": {
      "commandName": "IISExpress",
      "launchBrowser": true,
      "launchUrl": "swagger",
      "environmentVariables": {
        "ASPNETCORE_ENVIRONMENT": "Development"
      }
    }
  }
}
```



**TodoController.cs**
 - Now, the controller becomes a simple bridge to the service:

```csharp
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
    [HttpGet]
    public async Task<ActionResult<IEnumerable<TodoItem>>> GetTodos()
    {
        return await _todoService.GetTodos();
    }
    [HttpPost]
    public async Task<ActionResult<TodoItem>> AddTodo([FromBody] TodoItem item)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        var todo = await _todoService.AddTodo(item);
        return CreatedAtAction(nameof(GetTodos), new { id = todo.Id }, todo);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateTodo(int id, [FromBody] TodoItem item)
    {
        if (id != item.Id)
        {
            return BadRequest();
        }
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        await _todoService.UpdateTodo(item);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTodoById(int id)
    {
        await _todoService.DeleteTodoById(id);
        return NoContent();
    }
}
```

**Data**
```csharp
using Microsoft.EntityFrameworkCore;
using TodoApi.Models;
namespace TodoApi.Data
{
    public class TodoContext : DbContext
    {
        public TodoContext(DbContextOptions<TodoContext> options) : base(options)
        {
        }
        public DbSet<TodoItem> TodoItems { get; set; }
    }
}
```


ITodoService.cs **(Interface for the service)**

```csharp
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
```


TodoItem.cs **Model**

```
using System.ComponentModel.DataAnnotations;
namespace TodoApi.Models
{
    public class TodoItem
    {
        public int Id { get; set; } = 0;

        [Required(ErrorMessage = "Title is required.")]
        [StringLength(100, ErrorMessage = "Title length cannot exceed 100 characters.")]
        public string Title { get; set; }

        public bool IsCompleted { get; set; } = false;
    }
    public class TodoDto
    {
        public int Id { get; set; } = 0;        

        public bool IsCompleted { get; set; } = false;
    }
}
```



TodoService.cs **(Services)**

```csharp
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
```


**appsettings.json**
```
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*"
}
```

**Program.cs**

```
using Microsoft.EntityFrameworkCore;
using TodoApi.Data;
using TodoApi.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddScoped<ITodoService, TodoService>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
// Register the In-Memory database
builder.Services.AddDbContext<TodoContext>(opt => opt.UseInMemoryDatabase("TodoList"));
// Enable CORS to allow requests from the frontend
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});
var app = builder.Build();
// Configure the HTTP request pipeline.
app.UseCors("AllowAll");
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
```

4. **Performance: Optimizing API Calls and Rendering**
   - To improve performance in both backend and frontend:
     
**Backend**

**Asynchronous Programming:**
 - Use async/await to optimize IO-bound operations, which you’ve already done.
**Caching:** 
 - Implement caching (using in-memory cache, distributed cache, or Redis) if requests are repeated frequently.

**Frontend**

**Minimize API Calls:**
 - Batch API calls wherever possible instead of sending multiple requests.
**Client-side Rendering:**
 - Use frameworks like React/Angular for more efficient rendering of the UI. Use state management libraries (e.g., Redux) to avoid unnecessary re-renders.
**Pagination:** 
 - Implement pagination for large datasets to reduce the load on both client and server.

**Conclusion**

**Security:**
 - Input validation and sanitization protect the application from malicious data.

**Scalability:**
 - Thread safety is handled by EF Core, and in-memory state management is simplified.

**Maintainability:**
 - The application is organized into meaningful modules, with services handling business logic and controllers handling routing.

**Performance:**
 - Async calls and caching will improve API response times and reduce load.



----

## **Frontend: React**


**Maintainability Structure: Code Organization and Naming Conventions**

 - We should apply clean architecture principles such as separating concerns by organizing code into appropriate folders for Public,Source, Components, Services, Here's how to organize the project:
   

Here's a complete React frontend project that works with the ASP.NET Core Web API backend using an in-memory database. You can copy this project structure and code into your environment to run the frontend.

**Project Structure:**

- Here is the structure of your React frontend:

```
ReactTodo/
├── Public/
│   └──index.html
├── src/
│   └── Components/
│   │   └── TodoItem.js // Individual todo item
│   │   └── TodoList.js // Lists all todos
│   └── Services/
│   │   └── todoService.js // All API calls go here
│   ├── App.js
│   ├── index.js
├── package.json

```
**Step-by-Step: Creating the React Frontend**

1. **Initialize the React Application**

   - Start by creating a new React app if you haven't already:
  
```
npx create-react-app ReactTodo
cd ReactTodo
```

2. **Edit the package.json**

```
 {
  "name": "todolist-app",
  "version": "0.1.0",
  "private": true,
  "dependencies": {
    "@testing-library/jest-dom": "^5.17.0",
    "@testing-library/react": "^13.4.0",
    "@testing-library/user-event": "^13.5.0",
    "react": "^18.3.1",
    "react-dom": "^18.3.1",
    "react-scripts": "5.0.1",
    "web-vitals": "^2.1.4"
  },
  "scripts": {
    "start": "react-scripts start",
    "build": "react-scripts build",
    "test": "react-scripts test",
    "eject": "react-scripts eject"
  },
  "eslintConfig": {
    "extends": [
      "react-app",
      "react-app/jest"
    ]
  },
  "browserslist": {
    "production": [
      ">0.2%",
      "not dead",
      "not op_mini all"
    ],
    "development": [
      "last 1 chrome version",
      "last 1 firefox version",
      "last 1 safari version"
    ]
  }
}

```

3. **Create the Components and Services**
   
   - You will have two components, TodoList.js and TodoItem.js, and a service file for managing API calls.
     src/index.js
   - This file is the entry point of the React app, rendering the App component to the DOM.
  
```
 import React from 'react';
import ReactDOM from 'react-dom/client';
import App from './App';

const root = ReactDOM.createRoot(document.getElementById('root'));
root.render(
  <React.StrictMode>
    <App />
  </React.StrictMode>
);


```

**src/App.js**

- The App.js file is the main component of your application, responsible for fetching todos from the API and rendering the TodoList component.
  
**Security:**
  - Sanitize and Validate All User Inputs

```
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

```

**src/components/TodoList.js**

- This component is responsible for rendering the list of todos. It renders each todo item using the TodoItem component.

```
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

```

**src/components/TodoItem.js**

- The TodoItem.js file represents each individual todo item in the list. It provides buttons to toggle and delete the todo.

```
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

```

**src/services/todoService.js**

- This service handles all the API calls to your ASP.NET Core Web API. It communicates with the backend to get, create, update, and delete todos.
  
```
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

```

4. **Edit the public/index.html**

   - Your public/index.html is the template where the React app will be injected. You can customize it, but here's a simple version:
  
```
 <!DOCTYPE html>
<html lang="en">
  <head>
    <meta charset="utf-8" />
    <link rel="icon" href="%PUBLIC_URL%/favicon.ico" />
    <meta name="viewport" content="width=device-width, initial-scale=1" />
    <meta name="theme-color" content="#000000" />
    <meta
      name="description"
      content="Web site created using create-react-app"
    />
    <link rel="apple-touch-icon" href="%PUBLIC_URL%/logo192.png" />
    <!--
      manifest.json provides metadata used when your web app is installed on a
      user's mobile device or desktop. See https://developers.google.com/web/fundamentals/web-app-manifest/
    -->
    <link rel="manifest" href="%PUBLIC_URL%/manifest.json" />    
    <title>Todo App</title>
  </head>
  <body>
    <noscript>You need to enable JavaScript to run this app.</noscript>
    <div id="root"></div>    
  </body>
</html>


```

5. **Configure .gitignore**
   - To prevent unwanted files from being pushed to version control, here is a basic .gitignore:

```
 # See https://help.github.com/articles/ignoring-files/ for more about ignoring files.

# dependencies
/node_modules
/.pnp
.pnp.js

# testing
/coverage

# production
/build

# misc
.DS_Store
.env.local
.env.development.local
.env.test.local
.env.production.local

npm-debug.log*
yarn-debug.log*
yarn-error.log*

```

6. **README.md**


# Todo App

This is a simple Todo App frontend built with React, which interacts with an ASP.NET Core Web API backend.

## Available Scripts

In the project directory, you can run:

### `npm start`

Runs the app in the development mode.\
Open [http://localhost:3000](http://localhost:3000) to view it in the browser.

### `npm run build`

Builds the app for production to the `build` folder.

## Backend API

The frontend is expected to communicate with an ASP.NET Core Web API running on `http://localhost:5000`.

Ensure the backend is running before interacting with the frontend.


**Run the Project**

- Once you’ve set up the files, you can run your React project by doing the following:

1. **Install the dependencies:**
   ```
      npm install
   ```
2. **Start the development server:**
   ```
   npm start
   ```

Your React frontend will now be running on http://localhost:3000, and it will communicate with the **ASP.NET Core Web API** backend running on http://localhost:5000.

**Testing the App**

**Adding a Todo:**
- Enter a task in the input box and click Add.

**Toggling a Todo:**
- Click the Toggle button next to a task to mark it as complete/incomplete.

**Deleting a Todo:** 
- Click the Delete button to remove the task.

Now your **React frontend** is ready to interact with the **ASP.NET Core Web API** backend. You can further customize and improve this project as needed.


 












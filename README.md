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

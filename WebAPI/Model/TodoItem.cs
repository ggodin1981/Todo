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

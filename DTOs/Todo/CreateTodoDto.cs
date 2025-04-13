using System.ComponentModel.DataAnnotations;

namespace TodoApp.DTOs.Todo
{
    public class CreateTodoDto
    {
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateTime? DueDate { get; set; }
    }

}

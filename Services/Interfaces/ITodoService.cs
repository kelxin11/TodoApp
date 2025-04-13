using TodoApp.DTOs;
using TodoApp.DTOs.Todo;
using TodoApp.Models;

namespace TodoApp.Services.Interfaces
{
    public interface ITodoService
    {
        Task<List<TodoDto>> GetTodosAsync(int userId, string? search, string? sortBy, bool isDesc, int page, int pageSize);
        Task<TodoDto?> GetByIdAsync(int id, int userId);
        Task<TodoDto> CreateAsync(CreateTodoDto dto, int userId);
        Task<TodoDto> UpdateAsync(int id, UpdateTodoDto dto, int userId);
        Task DeleteAsync(int id, int userId);
        Task<TodoDto> ToggleCompleteAsync(int id, int userId);
    }



}

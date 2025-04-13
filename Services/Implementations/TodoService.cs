using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TodoApp.DTOs.Todo;
using TodoApp.Services.Interfaces;
using TodoApp.Data;
using TodoApp.Models;
using TodoApp.Exceptions;

public class TodoService : ITodoService
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;
    private readonly ILogger<TodoService> _logger;

    public TodoService(AppDbContext context, IMapper mapper, ILogger<TodoService> logger)
    {
        _context = context;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<List<TodoDto>> GetTodosAsync(int userId, string? search, string? sortBy, bool isDesc, int page, int pageSize)
    {
        try
        {
            var query = _context.Todos.Where(t => t.UserId == userId);

            if (!string.IsNullOrEmpty(search))
                query = query.Where(t => t.Title.Contains(search));

            if (!string.IsNullOrEmpty(sortBy))
                query = sortBy.ToLower() switch
                {
                    "title" => isDesc ? query.OrderByDescending(t => t.Title) : query.OrderBy(t => t.Title),
                    "duedate" => isDesc ? query.OrderByDescending(t => t.DueDate) : query.OrderBy(t => t.DueDate),
                    _ => query
                };

            var todos = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
            return _mapper.Map<List<TodoDto>>(todos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching todos for user {UserId}", userId);
            throw;
        }
    }

    public async Task<TodoDto?> GetByIdAsync(int id, int userId)
    {
        try
        {
            var todo = await _context.Todos.FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);
            return todo == null ? null : _mapper.Map<TodoDto>(todo);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching todo ID {TodoId} for user {UserId}", id, userId);
            throw;
        }
    }

    public async Task<TodoDto> CreateAsync(CreateTodoDto dto, int userId)
    {
        try
        {
            var todo = _mapper.Map<Todo>(dto);
            todo.UserId = userId;
            todo.CreatedDate = DateTime.UtcNow;

            _context.Todos.Add(todo);
            await _context.SaveChangesAsync();

            return _mapper.Map<TodoDto>(todo);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating todo for user {UserId}", userId);
            throw;
        }
    }

    public async Task<TodoDto> UpdateAsync(int id, UpdateTodoDto dto, int userId)
    {
        try
        {
            var todo = await _context.Todos.FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);
            if (todo == null)
                throw new NotFoundException("Todo not found");

            _mapper.Map(dto, todo);
            todo.UpdatedDate = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return _mapper.Map<TodoDto>(todo);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating todo ID {TodoId} for user {UserId}", id, userId);
            throw;
        }
    }

    public async Task DeleteAsync(int id, int userId)
    {
        try
        {
            var todo = await _context.Todos.FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);
            if (todo == null)
                throw new NotFoundException("Todo not found");

            _context.Todos.Remove(todo);
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting todo ID {TodoId} for user {UserId}", id, userId);
            throw;
        }
    }

    public async Task<TodoDto> ToggleCompleteAsync(int id, int userId)
    {
        try
        {
            var todo = await _context.Todos.FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);
            if (todo == null)
                throw new NotFoundException("Todo not found");

            todo.IsCompleted = !todo.IsCompleted;
            todo.UpdatedDate = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return _mapper.Map<TodoDto>(todo);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error toggling completion of todo ID {TodoId} for user {UserId}", id, userId);
            throw;
        }
    }
}

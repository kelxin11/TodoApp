using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TodoApp.DTOs.Todo;
using TodoApp.Models;
using TodoApp.Services.Interfaces;
/// <summary>
/// Controller for managing todo items.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TodosController : ControllerBase
{
    private readonly ITodoService _todoService;

    public TodosController(ITodoService todoService)
    {
        _todoService = todoService;
    }

    /// <summary>
    /// Gets the current user's ID from the JWT token.
    /// </summary>
    private int GetUserId() => int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

    /// <summary>
    /// Retrieves a list of todos for the authenticated user.
    /// </summary>
    /// <param name="search">Optional search query to filter todos.</param>
    /// <param name="sortBy">Optional field to sort by (e.g., "dueDate").</param>
    /// <param name="isDesc">Set to true to sort descending.</param>
    /// <param name="page">The page number for pagination.</param>
    /// <param name="pageSize">Number of items per page.</param>
    /// <returns>A list of todo items.</returns>
    [HttpGet]
    public async Task<IActionResult> GetTodos(
        string? search = null,
        string? sortBy = null,
        bool isDesc = false,
        int page = 1,
        int pageSize = 10)
    {
        var userId = GetUserId();
        var todos = await _todoService.GetTodosAsync(userId, search, sortBy, isDesc, page, pageSize);
        return Ok(todos);
    }

    /// <summary>
    /// Retrieves a specific todo item by ID.
    /// </summary>
    /// <param name="id">The ID of the todo item.</param>
    /// <returns>The matching todo item, or 404 if not found.</returns>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetTodoById(int id)
    {
        var userId = GetUserId();
        var todo = await _todoService.GetByIdAsync(id, userId);
        if (todo == null) return NotFound();
        return Ok(todo);
    }

    /// <summary>
    /// Creates a new todo item.
    /// </summary>
    /// <param name="dto">The data for the new todo item.</param>
    /// <returns>The created todo item.</returns>
    [HttpPost]
    public async Task<IActionResult> CreateTodo([FromBody] CreateTodoDto dto)
    {
        var userId = GetUserId();
        var created = await _todoService.CreateAsync(dto, userId);
        return CreatedAtAction(nameof(GetTodoById), new { id = created.Id }, created);
    }

    /// <summary>
    /// Updates an existing todo item.
    /// </summary>
    /// <param name="id">The ID of the todo to update.</param>
    /// <param name="dto">The updated todo data.</param>
    /// <returns>The updated todo item.</returns>
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateTodo(int id, [FromBody] UpdateTodoDto dto)
    {
        var userId = GetUserId();
        var updated = await _todoService.UpdateAsync(id, dto, userId);
        return Ok(updated);
    }

    /// <summary>
    /// Deletes a todo item.
    /// </summary>
    /// <param name="id">The ID of the todo to delete.</param>
    /// <returns>No content if successful.</returns>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTodo(int id)
    {
        var userId = GetUserId();
        await _todoService.DeleteAsync(id, userId);
        return NoContent();
    }

    /// <summary>
    /// Toggles the completion status of a todo item.
    /// </summary>
    /// <param name="id">The ID of the todo to toggle.</param>
    /// <returns>The updated todo item with new completion status.</returns>
    [HttpPatch("{id}/complete")]
    public async Task<IActionResult> ToggleComplete(int id)
    {
        var userId = GetUserId();
        var updated = await _todoService.ToggleCompleteAsync(id, userId);
        return Ok(updated);
    }
}

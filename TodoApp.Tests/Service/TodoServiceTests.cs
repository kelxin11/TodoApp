using AutoMapper;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using TodoApp.Data;
using TodoApp.DTOs.Todo;
using TodoApp.Exceptions;
using TodoApp.Mapping;

namespace TodoApp.Tests.Services
{
    public class TodoServiceTests
    {
        private readonly AppDbContext _context;
        private readonly TodoService _todoService;


        public TodoServiceTests()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new AppDbContext(options);

            var mapperConfig = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<MappingProfile>(); // Your AutoMapper profile
            });

            var logger = new NullLogger<TodoService>();

            var mapper = mapperConfig.CreateMapper();
            _todoService = new TodoService(_context, mapper, logger);
        }

        [Fact]
        public async Task CreateAsync_Should_Create_Todo()
        {
            // Arrange
            var dto = new CreateTodoDto
            {
                Title = "Test Todo",
                Description = "Test Desc",
                DueDate = DateTime.UtcNow.AddDays(1)
            };

            // Act
            var result = await _todoService.CreateAsync(dto, userId: 1);

            // Assert
            result.Should().NotBeNull();
            result.Title.Should().Be(dto.Title);
        }

        [Fact]
        public async Task UpdateAsync_Should_Update_Todo()
        {
            // Arrange
            var todo = await _todoService.CreateAsync(new CreateTodoDto
            {
                Title = "Original Title",
                Description = "Original Desc",
                DueDate = DateTime.UtcNow.AddDays(2)
            }, userId: 1);

            var updateDto = new UpdateTodoDto
            {
                Title = "Updated Title",
                Description = "Updated Desc",
                DueDate = DateTime.UtcNow.AddDays(5),
                IsCompleted = true
            };

            // Act
            var updated = await _todoService.UpdateAsync(todo.Id, updateDto, userId: 1);

            // Assert
            updated.Title.Should().Be("Updated Title");
            updated.Description.Should().Be("Updated Desc");
            updated.IsCompleted.Should().BeTrue();
        }

        [Fact]
        public async Task GetTodosAsync_Should_Return_Todos()
        {
            // Arrange
            await _todoService.CreateAsync(new CreateTodoDto
            {
                Title = "Todo 1",
                Description = "todo test",
                DueDate = DateTime.UtcNow.AddDays(1)
            }, userId: 1);

            await _todoService.CreateAsync(new CreateTodoDto
            {
                Title = "Todo 2",
                Description = "todo test",
                DueDate = DateTime.UtcNow.AddDays(2)
            }, userId: 1);

            // Act
            var todos = await _todoService.GetTodosAsync(userId: 1, search: null, sortBy: null, isDesc: false, page: 1, pageSize: 10);

            // Assert
            todos.Should().HaveCount(2);
        }

        [Fact]
        public async Task DeleteAsync_Should_Remove_Todo()
        {
            // Arrange
            var todo = await _todoService.CreateAsync(new CreateTodoDto
            {
                Title = "To Be Deleted",
                Description = "delete test",
                DueDate = DateTime.UtcNow.AddDays(1)
            }, userId: 1);

            // Act
            await _todoService.DeleteAsync(todo.Id, userId: 1);
            var result = await _todoService.GetByIdAsync(todo.Id, userId: 1);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task ToggleCompleteAsync_Should_Toggle_Completion()
        {
            // Arrange
            var todo = await _todoService.CreateAsync(new CreateTodoDto
            {
                Title = "Toggle Me",
                Description = "toggle test",
                DueDate = DateTime.UtcNow.AddDays(1)
            }, userId: 1);

            // Act
            var toggled = await _todoService.ToggleCompleteAsync(todo.Id, userId: 1);

            // Assert
            toggled.IsCompleted.Should().BeTrue();

            // Act again
            var toggledBack = await _todoService.ToggleCompleteAsync(todo.Id, userId: 1);

            // Assert again
            toggledBack.IsCompleted.Should().BeFalse();
        }

        [Fact]
        public async Task UpdateAsync_Should_Throw_NotFoundException_When_Todo_Does_Not_Exist()
        {
            // Arrange
            var updateDto = new UpdateTodoDto
            {
                Title = "Non-existent Todo",
                Description = "Should not exist",
                DueDate = DateTime.UtcNow.AddDays(1),
                IsCompleted = true
            };

            // Act & Assert
            Func<Task> act = async () => await _todoService.UpdateAsync(999, updateDto, userId: 1); // ID 999 should not exist
            await act.Should().ThrowAsync<NotFoundException>().WithMessage("Todo not found");
        }

        [Fact]
        public async Task DeleteAsync_Should_Throw_NotFoundException_When_Todo_Does_Not_Exist()
        {
            // Act & Assert
            Func<Task> act = async () => await _todoService.DeleteAsync(999, userId: 1); // ID 999 should not exist
            await act.Should().ThrowAsync<NotFoundException>().WithMessage("Todo not found");
        }

    }
}

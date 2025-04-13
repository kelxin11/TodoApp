using FluentValidation;
using TodoApp.DTOs.Todo;

namespace TodoApp.Validators;

public class TodoDtoValidator : AbstractValidator<TodoDto>
{
    public TodoDtoValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required");

        RuleFor(x => x.Description)
            .MaximumLength(500).WithMessage("Description can't exceed 500 characters");

        RuleFor(x => x.DueDate)
            .GreaterThan(DateTime.UtcNow).WithMessage("Due date must be in the future");
    }
}

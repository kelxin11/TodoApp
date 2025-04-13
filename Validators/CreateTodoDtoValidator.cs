using FluentValidation;
using TodoApp.DTOs.Todo;

namespace TodoApp.Validators;

public class CreateTodoDtoValidator : AbstractValidator<CreateTodoDto>
{
    public CreateTodoDtoValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required")
            .MaximumLength(100).WithMessage("Title can't be longer than 100 characters");

        RuleFor(x => x.Description)
            .MaximumLength(500).WithMessage("Description can't exceed 500 characters");

        RuleFor(x => x.DueDate)
            .GreaterThan(DateTime.UtcNow).WithMessage("Due date must be in the future");
    }
}

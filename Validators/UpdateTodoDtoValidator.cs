using FluentValidation;
using TodoApp.DTOs.Todo;

public class UpdateTodoDtoValidator : AbstractValidator<UpdateTodoDto>
{
    public UpdateTodoDtoValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required")
            .MaximumLength(100).WithMessage("Title can't be longer than 100 characters");

        RuleFor(x => x.Description)
            .MaximumLength(500).WithMessage("Description can't exceed 500 characters");

        RuleFor(x => x.DueDate)
            .NotNull().WithMessage("Due date is required")
            .GreaterThan(DateTime.UtcNow).WithMessage("Due date must be in the future");

        // This rule is fine but might not trigger if the client sends 'false'
        RuleFor(x => x.IsCompleted)
            .NotNull().WithMessage("Completion status is required");
    }
}

using Domain.Models;
using FluentValidation;

namespace Application.Validators;

public class TagValidator : AbstractValidator<Tag>
{
    public TagValidator()
    {
        RuleFor(p => p.Name)
            .NotNull().WithMessage("Name is required.")
            .NotEmpty().WithMessage("The tag name cannot be empty.")
            .MaximumLength(100).WithMessage("The tag name cannot be longer than 100 characters.");

        RuleFor(p => p.Description)
            .MaximumLength(2000).WithMessage("Tag description cannot be longer than 2000 characters.");
    }
}

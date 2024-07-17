using Domain.Models;
using FluentValidation;

namespace Application.Validators;

public class CommentValidator : AbstractValidator<Comment>
{
    public CommentValidator()
    {
        RuleFor(p => p.Content)
            .NotNull().WithMessage("Content is required.")
            .NotEmpty().WithMessage("Content cannot be empty.")
            .MaximumLength(10_000).WithMessage("Comment cannot be longer than 10000 characters.");
    }
}
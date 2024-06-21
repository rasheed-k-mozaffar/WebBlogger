using Domain.Models;
using FluentValidation;

namespace Application.Validators;

public class PostValidator : AbstractValidator<Post>
{
    public PostValidator()
    {
        RuleFor(p => p.Title)
            .NotEmpty().WithMessage("Title is required.")
            .Length(5, 1000).WithMessage("Title must be between 5 and 1,000 characters.");

        RuleFor(p => p.Content)
            .NotEmpty().WithMessage("Content is required.")
            .Length(10, 50_000).WithMessage("Content must be between 10 and 75,000 characters.");

        RuleFor(p => p.PublishedOn)
            .LessThanOrEqualTo(DateTime.Now).WithMessage("Post publication date can't be in the future.");

        RuleFor(p => p.Status)
            .IsInEnum().WithMessage("Invalid post status.");
    }
}
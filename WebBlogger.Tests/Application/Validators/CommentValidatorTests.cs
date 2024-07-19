using Application.Validators;
using FluentValidation;
using FluentValidation.TestHelper;

namespace WebBlogger.Tests.Application.Validators;

public class CommentValidatorTests
{
    private readonly IValidator<Comment> _validator = new CommentValidator();

    [Fact]
    public async Task Validate_ShouldHaveZeroErrors_WhenCommentIsValid()
    {
        // ARRANGE
        var comment = new Comment()
        {
            Content = "Valid comment content"
        };

        // ACT
        var result = await _validator.TestValidateAsync(comment);

        // ASSERT
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async Task Validate_ShouldHaveErrorsForContent_WhenContentIsEmpty()
    {
        // ARRANGE
        var invalidComment = new Comment()
        {
            Content = string.Empty
        };

        // ACT
        var result = await _validator.TestValidateAsync(invalidComment);

        // ASSERT
        result.ShouldHaveValidationErrorFor(x => x.Content);
    }

    [Fact]
    public async Task Validate_ShouldHaveErrorsForContent_WhenContentIsNull()
    {
        // ARRANGE
        var invalidComment = new Comment()
        {
            Content = null
        };

        // ACT
        var result = await _validator.TestValidateAsync(invalidComment);

        // ASSERT
        result.ShouldHaveValidationErrorFor(x => x.Content);
    }
}
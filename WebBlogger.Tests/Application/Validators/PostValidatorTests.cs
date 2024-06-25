using Application.Validators;
using FluentValidation;
using FluentValidation.TestHelper;

namespace WebBlogger.Tests.Application.Validators;

public class PostValidatorTests
{
    private readonly IValidator<Post> _postValidator;

    public PostValidatorTests()
    {
        _postValidator = new PostValidator();
    }

    [Fact]
    public async Task Validate_ShouldHaveTwoErrors_WhenBothTitleAndContentAreEmpty()
    {
        // arrange
        var invalidPost = new Post()
        {
            Id = Guid.NewGuid(),
            Title = "",
            Content = ""
        };

        // act
        var result = await _postValidator.TestValidateAsync(invalidPost);

        // assert
        result.ShouldHaveValidationErrorFor(p => p.Title);
        result.ShouldHaveValidationErrorFor(p => p.Content);
    }
}
using System.Text;
using Application.Validators;
using FluentValidation;
using FluentValidation.TestHelper;

namespace WebBlogger.Tests.Application.Validators;

public class TagValidatorTests
{
    private readonly IValidator<Tag> _tagValidator;

    public TagValidatorTests()
    {
        _tagValidator = new TagValidator();
    }

    [Fact]
    public async Task Validate_ShouldFailIfTagNameIsEmptyOrNull()
    {
        // ARRANGE
        var invalidTag = new Tag()
        {
            Name = ""
        };

        // ACT
        var result = await _tagValidator.TestValidateAsync(invalidTag);

        // ASSERT
        result.ShouldHaveValidationErrorFor(p => p.Name);
    }

    [Fact]
    public async Task Validate_ShouldFailIfTagNameIsLongerThan100Characters()
    {
        // ARRANGE
        StringBuilder builder = new StringBuilder();

        for (int i = 0; i < 101; i++)
        {
            builder.Append(i.ToString());
        }

        var invalidTagName = builder.ToString();
        var invalidTag = new Tag()
        {
            Name = invalidTagName
        };

        // ACT
        var result = await _tagValidator.TestValidateAsync(invalidTag);

        // ASSERT
        result.ShouldHaveValidationErrorFor(p => p.Name);
    }
}

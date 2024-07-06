using Application.Common.Interfaces;
using Application.Features.Tags.Commands;
using Application.Features.Tags.Handlers;
using FluentValidation;
using FluentValidation.Results;
using MediatR;

namespace WebBlogger.Tests.Application.Commands.Tags;

public class CreateTagCommandHandlerTests
{
    private readonly ITagsRepository _tagsRepository;
    private readonly IValidator<Tag> _tagValidator;
    private readonly CreateTagCommandHandler _handler;

    public CreateTagCommandHandlerTests()
    {
        _tagsRepository = Substitute.For<ITagsRepository>();
        _tagValidator = Substitute.For<IValidator<Tag>>();
        _handler = new CreateTagCommandHandler(_tagsRepository, _tagValidator);
    }

    [Fact]
    public async Task Handle_ShouldCreateTag_WhenCommandIsValid()
    {
        // ARRANGE
        var command = new CreateTagCommand()
        {
            Id = Guid.NewGuid(),
            Name = "C#",
            Description = "Valid Tag description",
            CoverImageUrl = null
        };

        var handler = new CreateTagCommandHandler(_tagsRepository, _tagValidator);
        var validationResult = new ValidationResult();

        _tagValidator.ValidateAsync(Arg.Any<Tag>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(validationResult));

        // ACT
        var result = await handler.Handle(command, CancellationToken.None);

        // ASSERT
        result.Should().Be(Unit.Value);
        await _tagValidator.Received(1).ValidateAsync(Arg.Any<Tag>(), Arg.Any<CancellationToken>());
        await _tagsRepository.Received(1).SaveTagAsync(Arg.Any<Tag>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ShouldThrowValidationException_WhenCommandIsInvalid()
    {
        // ARRANGE
        var command = new CreateTagCommand()
        {
            Id = Guid.NewGuid(),
            Name = string.Empty
        };
        var handler = new CreateTagCommandHandler(_tagsRepository, _tagValidator);

        var validationFailures = new List<ValidationFailure>()
        {
            new ValidationFailure("Name", "Tag name cannot be null or empty")
        };
        var validationResult = new ValidationResult(validationFailures);

        _tagValidator.ValidateAsync(Arg.Any<Tag>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(validationResult));

        // ACT
        Func<Task> action = async () => await handler.Handle(command, CancellationToken.None);

        // ASSERT
        await action.Should().ThrowAsync<ValidationException>();
        await _tagValidator.Received(1).ValidateAsync(Arg.Any<Tag>(), Arg.Any<CancellationToken>());
    }
}
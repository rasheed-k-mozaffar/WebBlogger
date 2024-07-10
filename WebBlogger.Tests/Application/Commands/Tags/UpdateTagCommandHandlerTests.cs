using Application.Common.Interfaces;
using Application.Common.Mappers;
using Application.Features.Tags.Commands;
using Application.Features.Tags.Handlers;
using Domain.Exceptions;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.Data;
using NSubstitute.ExceptionExtensions;

namespace WebBlogger.Tests.Application.Commands.Tags;

public class UpdateTagCommandHandlerTests
{
    private readonly ITagsRepository _tagsRepository;
    private readonly IValidator<Tag> _validator;
    private readonly UpdateTagCommandHandler _handler;

    public UpdateTagCommandHandlerTests()
    {
        _tagsRepository = Substitute.For<ITagsRepository>();
        _validator = Substitute.For<IValidator<Tag>>();
        _handler = new UpdateTagCommandHandler(_tagsRepository, _validator);
    }

    [Fact]
    public async Task Handle_ShouldUpdateTag_WhenTagIdExistsAndCommandIsValid()
    {
        // ARRANGE
        var tagId = Guid.NewGuid();
        var oldTag = new Tag()
        {
            Id = tagId,
            Name = "C#",
            Description = "Valid old tag description",
            CoverImageUrl = null
        };

        var command = new UpdateTagCommand()
        {
            Id = tagId,
            Name = "C Sharp",
            Description = "Valid new tag description",
            CoverImageUrl = null
        };

        var updatedTag = command.MapToTag();

        _tagsRepository.UpdateTagAsync(tagId, Arg.Any<Tag>(), CancellationToken.None)
            .Returns(updatedTag);

        var validationResult = new ValidationResult();
        _validator.ValidateAsync(Arg.Any<Tag>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(validationResult));
        // ACT
        var result = await _handler.Handle(command, CancellationToken.None);

        // ASSERT
        result.Should().NotBeNull();
        result.Name.Should().Be(command.Name);
        await _tagsRepository.Received(1).UpdateTagAsync(Arg.Any<Guid>(), Arg.Any<Tag>(), CancellationToken.None);
        await _validator.Received(1).ValidateAsync(Arg.Any<Tag>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ShouldThrowValidationException_WhenCommandIsInvalid()
    {
        // ARRANGE
        var command = new UpdateTagCommand()
        {
            Id = Guid.NewGuid(),
            Name = "",
            Description = null,
            CoverImageUrl = null
        };

        var updatedTag  = command.MapToTag();

        List<ValidationFailure> failures =
        [
            new ValidationFailure(nameof(Tag.Name), "The tag name cannot be empty")
        ];
        var validationResult = new ValidationResult(failures);

        _validator.ValidateAsync(Arg.Any<Tag>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(validationResult));

        // ACT
        Func<Task> action = async () => await _handler
            .Handle(command, CancellationToken.None);

        // ASSERT
        await action.Should().ThrowAsync<ValidationException>();
    }

    [Fact]
    public async Task Handle_ShouldThrowTagNotFoundException_WhenTagIdIsNotFound()
    {
        // ARRANGE
        _tagsRepository.UpdateTagAsync(Arg.Any<Guid>(), Arg.Any<Tag>(), Arg.Any<CancellationToken>())
            .ThrowsAsync(new TagNotFoundException("No tag was found with the given ID"));

        _validator.ValidateAsync(Arg.Any<Tag>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(new ValidationResult()));
        // ACT
        Func<Task> action = () => _handler
            .Handle(new UpdateTagCommand(), CancellationToken.None);

        // ASSERT
        await action.Should().ThrowAsync<TagNotFoundException>();
    }

    [Fact]
    public async Task Handle_ShouldThrowOperationCancelledException_WhenCancellationIsRequested()
    {
        // ARRANGE
        var cts = new CancellationTokenSource();
        _validator.ValidateAsync(Arg.Any<Tag>(), Arg.Any<CancellationToken>())
            .ThrowsAsync(new OperationCanceledException());

        // ACT
        await cts.CancelAsync();
        Func<Task> action = async () => await _handler.Handle(new UpdateTagCommand(), cts.Token);

        // ASSERT
        await action.Should().ThrowAsync<OperationCanceledException>();
    }
}